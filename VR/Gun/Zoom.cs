using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Zoom : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private GameObject scopeScreen;
    [SerializeField] private GameObject _cameraPos;

    private float zoomAmount = 3.0f; // 카메라를 앞으로 이동 시킬 거리
    private float speed = 5.0f;

    [SerializeField] private InputActionProperty LeftActivate;
    [SerializeField] private InputActionProperty RightSelect;

    private XRGrabInteractable XGI;
    private bool isGrabbed = false;

    private void Start()
    {
        XGI = GetComponentInParent<XRGrabInteractable>();
        _camera = GetComponent<Camera>();

        scopeScreen.SetActive(false);

        XGI.selectEntered.AddListener(OnGrab);
        XGI.selectExited.AddListener(OnReleased);
    }

    private void Update()
    {
        if (isGrabbed)
        {
            ZoomIn();
        }
    }

    private void ZoomIn()
    {
        Vector3 originPos = _cameraPos.transform.position; // 현재 _cameraPos의 위치


        if (LeftActivate.action.ReadValue<float>() > 0.5f && isGrabbed)
        {
            scopeScreen.SetActive(true);
            Vector3 zoomPos = originPos + _camera.transform.forward * zoomAmount;
            _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, zoomPos, speed * Time.deltaTime);
        }
        else
        {
            scopeScreen.SetActive(false);
            _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, originPos, speed * 1.5f * Time.deltaTime);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }
    private void OnReleased(SelectExitEventArgs arg)
    {
        isGrabbed = false;
    }

    private void OnDestroy()
    {
        if (XGI != null)
        {
            XGI.selectEntered.RemoveListener(OnGrab);
            XGI.selectExited.RemoveListener(OnReleased);
        }
    }
}
