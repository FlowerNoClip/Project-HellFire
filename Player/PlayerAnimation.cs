using UnityEngine;
using UnityEngine.Animations.Rigging;
using FishNet.Object;

public class PlayerAnimation : NetworkBehaviour
{
    public MovementState state;
    [SerializeField] private Animator anim;
    [SerializeField] private TwoBoneIKConstraint handIk;
    [SerializeField] private GameObject ak;
    [SerializeField] private GameObject talon;

    public enum MovementState
    {
        Idle,
        Walking,
        Sprinting,
        Crouching,
        CWalking,
        Air,
        Dead
    }

    public void SetIdle()
    {
        state = MovementState.Idle;
        SetAnimation();
    }

    public void SetWalk()
    {
        state = MovementState.Walking;
        SetAnimation();
    }

    public void SetRun()
    {
        state = MovementState.Sprinting;
        SetAnimation();
    }

    public void SetCrouch()
    {
        state = MovementState.Crouching;
        SetAnimation();
    }

    public void SetCWalk()
    {
        state = MovementState.CWalking;
        SetAnimation();
    }
    public void SetAir()
    {
        state = MovementState.Air;
        SetAnimation();
    }

    public void SetBodyWeightZero()
    {
        anim.SetLayerWeight(1, 0);
    }
    public void SetBodyWeightOne()
    {
        anim.SetLayerWeight(1, 1);
    }
    public void SetDead()
    {
        state = MovementState.Dead;
        SetAnimation();
    }
    public void SetXY(float _xDir, float _yDir)
    {
        anim.SetFloat("xDir", _xDir);
        anim.SetFloat("yDir", _yDir);
    }

    public void SetMainGun()
    {
        anim.SetBool("Rifle", true);
        anim.SetBool("Pistol", false);
        SetActiveMainGun();
        SetWeightOne();
    }
    public void SetSubGun()
    {
        anim.SetBool("Pistol", true);
        anim.SetBool("Rifle", false);
        SetActiveSubGun();
        SetWeightZero();
    }
    public void SetKnife()
    {
    }
    private void SetActiveMainGun()
    {
        if (!base.IsOwner)
        {
            ak.SetActive(true);
            talon.SetActive(false);
        }
        
    }
    private void SetActiveSubGun()
    {
        if(!base.IsOwner)
        {
            talon.SetActive(true);
            ak.SetActive(false);
        }
        
    }
    private void SetWeightOne()
    {
        handIk.weight = 1;
    }
    private void SetWeightZero()
    {
        handIk.weight = 0;
    }

    private void SetAnimation()
    {
        switch (state)
        {
            case MovementState.Idle:
                anim.SetBool("Idle", true);
                anim.SetBool("CWalk", false);
                anim.SetBool("Crouch", false);
                anim.SetBool("Walk", false);
                anim.SetBool("Run", false);
                anim.SetBool("Jump", false);
                anim.SetBool("Dead", false);
                break;
            case MovementState.Walking:
                anim.SetBool("Walk", true);
                anim.SetBool("CWalk", false);
                anim.SetBool("Crouch", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Run", false);
                anim.SetBool("Jump", false);
                anim.SetBool("Dead", false);
                break;
            case MovementState.Sprinting:
                anim.SetBool("Run", true);
                anim.SetBool("CWalk", false);
                anim.SetBool("Crouch", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", false);
                anim.SetBool("Jump", false);
                anim.SetBool("Dead", false);
                break;
            case MovementState.Crouching:
                anim.SetBool("Crouch", true);
                anim.SetBool("CWalk", false);
                anim.SetBool("Run", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", false);
                anim.SetBool("Jump", false);
                anim.SetBool("Dead", false);
                break;
            case MovementState.CWalking:
                anim.SetBool("CWalk", true);
                anim.SetBool("Crouch", false);
                anim.SetBool("Run", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", false);
                anim.SetBool("Jump", false);
                anim.SetBool("Dead", false);
                break;
            
            case MovementState.Air:
                anim.SetBool("Jump", true);
                anim.SetBool("CWalk", false);
                anim.SetBool("Crouch", false);
                anim.SetBool("Run", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", false);
                anim.SetBool("Dead", false);
                break;
            case MovementState.Dead:
                anim.SetBool("Dead", true);
                anim.SetBool("Jump", false);
                anim.SetBool("CWalk", false);
                anim.SetBool("Crouch", false);
                anim.SetBool("Run", false);
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", false);
                break;
        }
    }
    

}
