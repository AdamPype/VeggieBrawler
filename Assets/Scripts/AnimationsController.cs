using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsController {

    private Animator _animator;
    private PlayerScript _player;

    private int _zMovementAnimationParameter = Animator.StringToHash("ZMovement");
    private int _xMovementAnimationParameter = Animator.StringToHash("XMovement");

    private int _isGroundedAnimationParameter = Animator.StringToHash("IsGrounded");
    private int _distanceFromGroundParameter = Animator.StringToHash("DistanceFromGround");
    private int _verticalVelocityAnimationParameter = Animator.StringToHash("VerticalVelocity");

    private int _punchParameter = Animator.StringToHash("Punch");
    private int _takeDamageAnimationParameter = Animator.StringToHash("TakeDamage");

    private int _deathParameter = Animator.StringToHash("Die");
    private int _resetParameter = Animator.StringToHash("Reset");


    public AnimationsController(Animator animator, PlayerScript player)
    {
        _animator = animator;
        _player = player;
    }

    public void Update()
    {
        _animator.SetFloat(_zMovementAnimationParameter, _player._inputMovement.z);
        _animator.SetFloat(_xMovementAnimationParameter, _player._inputMovement.x);

        _animator.SetBool(_isGroundedAnimationParameter, _player.IsGrounded());
        _animator.SetFloat(_distanceFromGroundParameter, _player.GetDistanceFromGround());
        _animator.SetFloat(_verticalVelocityAnimationParameter, _player.GetVelocity().y);
    }

    public void Punch()
    {
        _animator.SetTrigger(_punchParameter);
    }

    public void TakeDamage()
    {
        _animator.SetTrigger(_takeDamageAnimationParameter);
    }

    public void Die()
    {
        _animator.SetTrigger(_deathParameter);
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        _animator.SetLayerWeight(layerIndex, weight);
    }

    public void ResetAnimations()
    {
        _animator.SetTrigger(_resetParameter);
        //other parameters are reset on update
    }

    public void ApplyRootMotion(bool apply)
    {
        _animator.applyRootMotion = apply;
    }
}
