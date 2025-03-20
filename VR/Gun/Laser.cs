using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent (typeof (LineRenderer))]

public class Laser : MonoBehaviour
{
    [SerializeField] private InputActionProperty XBtn;
    [SerializeField] private InputActionProperty RightSelect;
    private XRGrabInteractable XGI = null;
    private LineRenderer lr;

    private bool isGrab = false;

    private void Start()
    {
        XGI = GetComponentInParent<XRGrabInteractable>();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;

        XGI.selectEntered.AddListener(OnGrabbed);
        XGI.selectExited.AddListener(OnReleased);
    }

    private void Update()
    {
        if (XBtn.action.ReadValue<float>() > 0.9f && isGrab)
        {
            lr.enabled = !lr.enabled;
            XGI.activated.AddListener(OnOff);
        }
    }

    private void OnOff(ActivateEventArgs arg)
    {
        if (lr.enabled)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                lr.SetPosition(1, new Vector3(0, 0, hit.distance));
            }
            else
            {
                lr.SetPosition(1, new Vector3(0, 0, 50));
            }
        }
    }

    private void OnGrabbed(SelectEnterEventArgs arg)
    {
        isGrab = true;
    }

    private void OnReleased(SelectExitEventArgs arg)
    {
        isGrab = false;
    }
}
