using UnityEngine;
using UnityEngine.InputSystem;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Transporting;

public class PlayerController_temp2: NetworkBehaviour
{
    [SerializeField] CapsuleCollider capcol;

    [Header("KeyBinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode walkKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private string currentSpeed;
    [SerializeField] private float walkACC;
    [SerializeField] private float runACC;
    [SerializeField] private float maxWalkSPD;
    [SerializeField] private float maxRunSPD;
    [SerializeField] private float limitSPD;

    [Header("Jump")]
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool readyToJump;

    [Header("Crouch")]
    [SerializeField] private float crouchWalkACC;
    [SerializeField] private float maxCrouchWalkSPD; // ������������
    [SerializeField] private float crouchYscale;
    [SerializeField] private float startYscale;
    [SerializeField] private float crouchCooldown;
    [SerializeField] private bool readyCrouch;
    [SerializeField] private bool isCrouch;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool grounded;
    [SerializeField] private float groundDrag;
    [SerializeField] private float rayDistance;

    [Header("Slope Handler")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private RaycastHit slopeHit;
    [SerializeField] private bool onSolope;
    [SerializeField] private float slopeRayDistance;
    private bool exitSlope;


    [Header("Animation Smoothness")]
    [SerializeField] float inputLerpSpeed = 5f;
    private float smoothHorizontalInput;
    private float smoothVerticalInput;

    [SerializeField] private Transform orientation;
    [SerializeField]private Animator anim;
    private Rigidbody rb;
    private Vector3 moveDir;
    private float horizontalInput;
    private float verticalInput;

    public MovementState state;

    private float cameraYOffset = 0.4f;
    private Camera playerCamera;
    public enum MovementState
    {
        Idle,
        Walking,
        Sprinting,
        Crouching,
        Air
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            gameObject.GetComponent<PlayerController_temp2>().enabled = false;
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    public override void OnSpawnServer(NetworkConnection connection)
    {
        OnClientConnected(connection);
        /* �� �ݹ��� �� ��ü�� ���� ���� �޽����� Ŭ���̾�Ʈ�� ���۵� �Ŀ� �߻��մϴ�.
        * ���� ���, �� ��ü�� �ټ� Ŭ���̾�Ʈ���� ���� ���, �� �ݹ��� �ټ� �� �߻��ϸ�,
        * �� Ŭ���̾�Ʈ�� ���� connection �Ű������� ���޵˴ϴ�.
        * �ַ� �� �ݹ��� ����Ͽ� ��ü�� �����Ǵ� Ŭ���̾�Ʈ���� ������ ����� ���� �� ����մϴ�. */
    }


    private void OnClientConnected(NetworkConnection conn)
    {
        // ���ο� Ŭ���̾�Ʈ�� ������ �� ��� Ŭ���̾�Ʈ�� RPC ȣ��
        RpcApplySettings();
    }

    [ObserversRpc]
    private void RpcApplySettings()
    {
        // Ŭ���̾�Ʈ���� ����� �Լ�
        ApplySettings();
    }

    private void ApplySettings()
    {
        // �� �Լ��� ��� Ŭ���̾�Ʈ���� ����˴ϴ�.
        if (rb != null && capcol != null)
        {
            rb.freezeRotation = true;
            startYscale = capcol.height;
            Debug.Log("Settings applied on client.");
        }
        else
        {
            Debug.LogError("Rigidbody or CapsuleCollider is not assigned.");
        }
    }

    private void Update()
    {


        if (playerCamera != null)
        {
            // Ground Check
            Vector3 box = new Vector3(capcol.radius, 0.05f, capcol.radius);
            grounded = Physics.BoxCast(capcol.bounds.center, box, -transform.up, transform.rotation, capcol.height / 2 + rayDistance, whatIsGround);

            MyInput();
            StateHandler();
            MovePlayer();
            SpeedControl();

            if (grounded)
                rb.linearDamping = groundDrag;
            else
                rb.linearDamping = 0;

            currentSpeed = rb.linearVelocity.magnitude.ToString("F2");
        }

        


    }
    private void MyInput()
    {

        // Lerp�� ����Ͽ� smooth input ���� ������Ʈ�մϴ�.
        smoothHorizontalInput = Mathf.Lerp(smoothHorizontalInput, horizontalInput, Time.deltaTime * inputLerpSpeed);
        smoothVerticalInput = Mathf.Lerp(smoothVerticalInput, verticalInput, Time.deltaTime * inputLerpSpeed);

        if (Input.GetKeyDown(jumpKey)&& readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke("ResetJump", jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey) && grounded && readyCrouch)
        {
            capcol.height = capcol.height * crouchYscale;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            readyCrouch = false;
            isCrouch = true;
        }
        else if(Input.GetKeyDown(crouchKey) && !grounded && readyCrouch)
        {
            capcol.height = capcol.height * crouchYscale;
            readyCrouch = false;
            isCrouch = true;
        }

        if (Input.GetKeyUp(crouchKey) && isCrouch)
        {
            capcol.height = startYscale;
            isCrouch = false;
            Invoke("ResetCrouch", crouchCooldown);
        }
    }
    private void OnMove(InputValue value)
    {
        // input ���� �����ɴϴ�.
        Vector2 input = value.Get<Vector2>();
        if(input != null)
        {
            horizontalInput = input.x;
            verticalInput = input.y;
        }
    }

    private void StateHandler()
    {
        Vector2 movement = new Vector2(smoothHorizontalInput, smoothVerticalInput);
        movement.Normalize();
        anim.SetFloat("xDir", movement.x);
        anim.SetFloat("yDir", movement.y);
        // Mode - Crouch
        if (isCrouch)
        {
            moveSpeed = crouchWalkACC;
            limitSPD = maxCrouchWalkSPD;
            state = MovementState.Crouching;
            return;
        }

        // Mode - Sprint
        if(Input.GetKey(walkKey)&& grounded)
        {
            anim.SetBool("Walk", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Run", false);
            anim.SetBool("Jump", false);
            moveSpeed = walkACC;
            limitSPD = maxWalkSPD;
            state = MovementState.Walking;
        }
        // Mode - Walk
        else if(grounded && (horizontalInput !=0 || verticalInput !=0))
        {
            anim.SetBool("Run", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Jump", false);
            moveSpeed = runACC;
            limitSPD = maxRunSPD;
            state = MovementState.Sprinting;
        }
        // Mode - Idle
        else if (grounded && Mathf.Approximately(horizontalInput, 0) && Mathf.Approximately(verticalInput, 0)) 
        {
            anim.SetBool("Idle", true);
            anim.SetBool("Walk",false);
            anim.SetBool("Run",false);
            anim.SetBool("Jump",false);
            state = MovementState.Idle;
        }
        // Mode - Air
        else if(!grounded)
        {
            anim.SetBool("Jump",true);
            state = MovementState.Air;
        }
    }

    private void MovePlayer()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (OnSlope() && !exitSlope)
        {
            Debug.Log(GetSlopeMoveDirection() + " Slope Direction");
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down*80f, ForceMode.Force);
            }
        }
        if (grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        }
        else if(!grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }
    private void Jump()
    {
        exitSlope = true;
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * maxJumpHeight, ForceMode.Impulse);
    }


    private void ResetJump()
    {
        readyToJump = true;
        exitSlope = false;
    }

    private void ResetCrouch()
    {
        readyCrouch = true;
    }

    private void SpeedControl()
    {
        // ���ο��� �ӵ�����
        if (OnSlope() && !exitSlope)
        {
            if(rb.linearVelocity.magnitude > limitSPD)
            {
                Debug.Log("���� �ӵ� ����");
                rb.linearVelocity = rb.linearVelocity.normalized * limitSPD;
            }
        }
        // ���� �� �ӵ�����( ������ �� or ����)
        else
        { 
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > limitSPD)
            {
                LimitSpeed(flatVel);
            }   
        }  
    }

    private void LimitSpeed(Vector3 _flatVel)
    {
        Vector3 limitVel = _flatVel.normalized * limitSPD;
        rb.linearVelocity = new Vector3(limitVel.x, rb.linearVelocity.y, limitVel.z);
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(capcol.bounds.center,Vector3.down, out slopeHit, capcol.height/2 + slopeRayDistance ,whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }
    

    private void OnDrawGizmos()
    {
        RaycastHit hit;
        Vector3 box = new Vector3(capcol.radius, 0.05f, capcol.radius);
        bool isHit = Physics.BoxCast(capcol.bounds.center, box, -transform.up, out hit, transform.rotation, capcol.height / 2 + rayDistance, whatIsGround);
        Gizmos.color = Color.red;
        if (isHit)
        {
            Gizmos.DrawRay(capcol.bounds.center, -transform.up * hit.distance);
            Gizmos.DrawWireCube(capcol.bounds.center + (-transform.up * hit.distance), box);
        }
        else
        {
            Gizmos.DrawRay(capcol.bounds.center, -transform.up * (capcol.height / 2 + rayDistance));
        }
    }
}
