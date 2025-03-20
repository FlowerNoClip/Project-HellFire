using UnityEngine;

public class PlayerColliderManger : MonoBehaviour
{
    [SerializeField] private Collider head;
    [SerializeField] private Collider chest;
    [SerializeField] private Collider leftUpperArm;
    [SerializeField] private Collider leftForeArm;
    [SerializeField] private Collider rightUpperArm;
    [SerializeField] private Collider rightForeArm;
    [SerializeField] private Collider stomach;
    [SerializeField] private Collider hip;
    [SerializeField] private Collider leftUpperLeg;
    [SerializeField] private Collider leftLowerLeg;
    [SerializeField] private Collider rightUpperLeg;
    [SerializeField] private Collider rightLowerLeg;

    public void DeActivveColliderAll()
    {
        head.enabled = false;
        chest.enabled = false;
        leftUpperArm.enabled = false;
        leftForeArm.enabled = false;
        rightUpperLeg.enabled = false;
        rightLowerLeg.enabled = false;
        stomach.enabled = false;
        hip.enabled = false;
        leftUpperLeg.enabled = false;
        leftLowerLeg.enabled = false;
        rightUpperLeg.enabled = false;
        rightLowerLeg.enabled = false;
    }

    public void ActiveColliderAll()
    {
        head.enabled = true;
        chest.enabled = true;
        leftUpperArm.enabled = true;
        leftForeArm.enabled = true;
        rightUpperLeg.enabled = true;
        rightLowerLeg.enabled = true;
        stomach.enabled = true;
        hip.enabled = true;
        leftUpperLeg.enabled = true;
        leftLowerLeg.enabled = true;
        rightUpperLeg.enabled= true;
        rightLowerLeg.enabled= true;
    }

    
}
