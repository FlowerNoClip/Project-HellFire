using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty ActivateValue;
    public InputActionProperty SelectValue;

    public Animator handAnimator;

    private void Update()
    {
        float triggerValue = ActivateValue.action.ReadValue<float>();
        float gripValue = SelectValue.action.ReadValue<float>();

        handAnimator.SetFloat("Trigger", triggerValue);
        handAnimator.SetFloat("Grip", gripValue);
    }
}
