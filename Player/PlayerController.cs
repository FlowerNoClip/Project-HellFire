using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;
using FishNet.Connection;
using System.Collections;
using FishNet.Example.ColliderRollbacks;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerScreenEffect playerScreenEffect;
    [SerializeField]
    private ItemManager2 itemManager2;

    private Vector3 RecoilPattern = Vector3.zero;

    [SerializeField] private CapsuleCollider capcol;
    [SerializeField] private GameObject target;
    public bool shopCont = false;

    // Movement
    private float moveSpeed;
    [SerializeField] private float limitSPD;
    [SerializeField] private bool isWalk;

    // Jump
    [SerializeField] private bool readyToJump;
    [SerializeField] private float airMultiplier;

    // Crouch
    [SerializeField] private float crouchYscale;
    [SerializeField] private float startYscale;
    [SerializeField] private bool readyCrouch;
    [SerializeField] private bool isCrouch;
    private Vector3 originalCenter;

    [Header("Ground Check")]
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
    [SerializeField] private PlayerAnimation pAnim;
    private float smoothHorizontalInput;
    private float smoothVerticalInput;


    [Header("Camera")]
    [SerializeField] private float cameraYOffset = 1.8f;
    public Camera playerCamera;
    private float xRotation;
    private float yRotation;


    private Rigidbody rb;
    private Vector3 moveDir;
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private GameObject gunPre;
    [SerializeField] private GameObject myBody;
    [SerializeField] public GameObject myGun;

    //Î∞òÎèô Lerp?ö©
    private Vector3 currentRecoil = Vector3.zero;
    private Vector3 targetRecoil = Vector3.zero;
    private bool applyingRecoil = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
    }

    private void Start()
    {
        rb.freezeRotation = true;
        startYscale = capcol.height;
        originalCenter = capcol.center;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
            playerCamera.nearClipPlane = 0.01f;
            GameObject g = Instantiate(gunPre, playerCamera.transform.position, Quaternion.identity);
            InputSystem inputSystem = transform.GetComponent<InputSystem>();
            myBody.SetActive(false);
            myGun.SetActive(false);

            itemManager2 = g.GetComponent<ItemManager2>();

            inputSystem._IM2 = itemManager2;
            playerScreenEffect._IM2 = itemManager2;
            //itemManager2.AdrenalineCallback = playerScreenEffect.Adrenaline;
            g.transform.SetParent(playerCamera.transform);

        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }
    public override void OnSpawnServer(NetworkConnection connection)
    {
        OnClientConnected(connection);
        /* ?ù¥ ÏΩúÎ∞±??? ?ù¥ Í∞ùÏ≤¥?óê ????ïú ?ä§?è∞ Î©îÏãúÏß?Í∞? ?Å¥?ùº?ù¥?ñ∏?ä∏Î°? ?†Ñ?Ü°?êú ?õÑ?óê Î∞úÏÉù?ï©?ãà?ã§.
        * ?òàÎ•? ?ì§?ñ¥, ?ù¥ Í∞ùÏ≤¥Í∞? ?ã§?ÑØ ?Å¥?ùº?ù¥?ñ∏?ä∏?óêÍ≤? Î≥¥Ïùº Í≤ΩÏö∞, ?ù¥ ÏΩúÎ∞±??? ?ã§?ÑØ Î≤? Î∞úÏÉù?ïòÎ©?,
        * Í∞? ?Å¥?ùº?ù¥?ñ∏?ä∏?óê ????ïú connection Îß§Í∞úÎ≥??àòÍ∞? ?†Ñ?ã¨?ê©?ãà?ã§.
        * Ï£ºÎ°ú ?ù¥ ÏΩúÎ∞±?ùÑ ?Ç¨?ö©?ïò?ó¨ Í∞ùÏ≤¥Í∞? ?ä§?è∞?êò?äî ?Å¥?ùº?ù¥?ñ∏?ä∏?óêÍ≤? ÎßûÏ∂§?òï ?Üµ?ã†?ùÑ Î≥¥ÎÇº ?ïå ?Ç¨?ö©?ï©?ãà?ã§. */
    }

    private void OnClientConnected(NetworkConnection conn)
    {
        // ?ÉàÎ°úÏö¥ ?Å¥?ùº?ù¥?ñ∏?ä∏Í∞? ?†ë?Üç?ï† ?ïå Î™®Îì† ?Å¥?ùº?ù¥?ñ∏?ä∏?óê RPC ?ò∏Ï∂?
        RpcApplySettings();
    }

    [ObserversRpc]
    private void RpcApplySettings()
    {
        // ?Å¥?ùº?ù¥?ñ∏?ä∏?óê?Ñú ?ã§?ñâ?ê† ?ï®?àò
        ApplySettings();
    }

    private void ApplySettings()
    {
        // ?ù¥ ?ï®?àò?äî Î™®Îì† ?Å¥?ùº?ù¥?ñ∏?ä∏?óê?Ñú ?ã§?ñâ?ê©?ãà?ã§.
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
        if (!player.isDead && playerCamera != null && shopCont == false)
        {
            // Ground Check
            Vector3 box = new Vector3(capcol.radius, 0.05f, capcol.radius);
            grounded = Physics.BoxCast(capcol.bounds.center, box, -transform.up, transform.rotation, capcol.height / 2 + rayDistance, whatIsGround);

            StateHandler();
            Rotate();
            SpeedControl();

            if (grounded)
                rb.linearDamping = groundDrag;
            else
                rb.linearDamping = 0;

            player.CurrentSpeed = rb.linearVelocity.magnitude;
        }
    }

    private void FixedUpdate()
    {
        if (!player.isDead && playerCamera != null && shopCont == false)
        {
            MovePlayer();
        }
    }
    private void OnMove(InputValue value)
    {
        // input Í∞íÏùÑ Í∞??†∏?òµ?ãà?ã§.
        Vector2 input = value.Get<Vector2>();
        if (input != null)
        {
            horizontalInput = input.x;
            verticalInput = input.y;
        }
    }

    private void StateHandler()
    {
        // LerpÎ•? ?Ç¨?ö©?ïò?ó¨ smooth input Í∞íÏùÑ ?óÖ?ç∞?ù¥?ä∏?ï©?ãà?ã§.
        smoothHorizontalInput = Mathf.Lerp(smoothHorizontalInput, horizontalInput, Time.deltaTime * inputLerpSpeed);
        smoothVerticalInput = Mathf.Lerp(smoothVerticalInput, verticalInput, Time.deltaTime * inputLerpSpeed);
        Vector2 movement = new Vector2(smoothHorizontalInput, smoothVerticalInput);
        movement.Normalize();
        pAnim.SetXY(movement.x, movement.y);

        bool isInputZero = Mathf.Abs(horizontalInput) < 0.01f && Mathf.Abs(verticalInput) < 0.01f;

        if (grounded)
        {
            if (isCrouch)
            {
                if (isInputZero)
                {
                    Debug.Log("?ïâÍ∏? Idle");
                    pAnim.SetCrouch();
                }
                else
                {
                    Debug.Log("?ïâÍ∏? ???ÏßÅÏûÑ");
                    moveSpeed = player.CrouchWalkACC;
                    limitSPD = player.MaxCrouchWalkSPD;
                    pAnim.SetCWalk();
                }
            }
            else
            {
                if (isInputZero)
                {
                    pAnim.SetIdle();
                }
                else if (!isWalk)
                {
                    moveSpeed = player.RunACC;
                    limitSPD = player.MaxRunSPD;
                    pAnim.SetRun();
                }
            }
        }
        else
        {
            pAnim.SetAir();
        }
    }

    private void SetRecoil(Vector3 Recoil)
    {
        Debug.Log("Recoil!" + Recoil);
        targetRecoil = Recoil;
        if (!applyingRecoil)
        {
            StartCoroutine(ApplyRecoil());
        }
    }
    private IEnumerator ApplyRecoil()
    {
        applyingRecoil = true;
        while (Vector3.Distance(currentRecoil, targetRecoil) > 0.1f)
        {
            currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * 10f);
            yield return null;
        }
        currentRecoil = targetRecoil;

        targetRecoil = Vector3.zero;
        currentRecoil = Vector3.zero;

        applyingRecoil = false;
    }


    private void Rotate()
    {

        if (itemManager2.ReturnCurrentGun() != null)
        {
            itemManager2.ReturnCurrentGun().RecoilCallback = SetRecoil;
        }
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * player.SenX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * player.SenY;
        yRotation += mouseX + currentRecoil.y;
        xRotation += -mouseY + currentRecoil.x;
        xRotation = Mathf.Clamp(xRotation, -60f, 90f);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        target.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 8f;

        RecoilPattern = Vector3.zero;
    }



    private void MovePlayer()
    {
        moveDir = (transform.forward * verticalInput + transform.right * horizontalInput) * Time.deltaTime;

        if (OnSlope() && !exitSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 3f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    public void Walk()
    {
        if (grounded && !isCrouch)
        {
            pAnim.SetWalk();
            moveSpeed = player.WalkACC;
            limitSPD = player.MaxWalkSPD;
            isWalk = true;
        }
    }
    public void StopWalk()
    {
        isWalk = false;
    }
    public void Jump()
    {

        if (readyToJump && grounded)
        {
            if (isCrouch)
            {
                StandUp();
            }
            readyToJump = false;
            exitSlope = true;
            // reset y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(transform.up * player.MaxJumpHeight, ForceMode.Impulse);
            Invoke("ResetJump", player.JumpCooldown);
        }
    }


    private void ResetJump()
    {
        readyToJump = true;
        exitSlope = false;
    }

    public void Crouch()
    {
        if (readyCrouch)
        {
            StopAllCoroutines();
            StartCoroutine(StartCrouch());
        }

    }
    public IEnumerator StartCrouch()
    {
        // ?ïÖ?óê?Ñú ?ïâ?ïò?ùÑ Í≤ΩÏö∞
        float elapsedTime = 0;
        float duration = 0.35f;
        float startHeight = capcol.height;
        float targetHeight = startYscale * crouchYscale;
        Vector3 startCenter = capcol.center;
        Vector3 targetCenter = originalCenter - new Vector3(0, (startYscale - targetHeight) / 2, 0);
        isCrouch = true;
        readyCrouch = false;
        while (elapsedTime < duration)
        {
            capcol.center = Vector3.Lerp(startCenter, targetCenter, elapsedTime / duration);
            capcol.height = Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration);
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration), transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        capcol.center = targetCenter;
        capcol.height = targetHeight;
    }

    public void StandUp()
    {
        if (isCrouch)
        {
            StopAllCoroutines();
            Invoke("ResetCrouch", player.CrouchCooldown);
            StartCoroutine(StartStand());
        }
    }

    private IEnumerator StartStand()
    {
        isCrouch = false;
        float elapsedTime = 0;
        float duration = 0.35f;
        float startHeight = capcol.height;
        float targetHeight = startYscale;
        Vector3 startCenter = capcol.center;
        Vector3 targetCenter = originalCenter;
        while (elapsedTime < duration)
        {
            capcol.center = Vector3.Lerp(startCenter, targetCenter, elapsedTime / duration);
            capcol.height = Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration);
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration), transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        capcol.height = startYscale;
        capcol.center = originalCenter;
    }

    private void ResetCrouch()
    {
        readyCrouch = true;
    }
    private void SpeedControl()
    {
        // Í≤ΩÏÇ¨Î°úÏóê?Ñú ?Üç?èÑ?†ú?ïú
        if (OnSlope() && !exitSlope)
        {
            if (rb.linearVelocity.magnitude > limitSPD)
            {
                Debug.Log("Í≤ΩÏÇ¨Î°? ?Üç?èÑ ?†ú?ïú");
                rb.linearVelocity = rb.linearVelocity.normalized * limitSPD;
            }
        }
        // Í≤ΩÏÇ¨Î°? ?ô∏ ?Üç?èÑ?†ú?ïú( ?èâ?èâ?ïú ?ïÖ or Í≥µÏ§ë)
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
        if (Physics.Raycast(capcol.bounds.center, Vector3.down, out slopeHit, capcol.height / 2 + slopeRayDistance, whatIsGround))
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