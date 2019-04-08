using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PhysicsController))]
public class PlayerScript : MonoBehaviour
{
    [SerializeField] [Range(1, 2)] private int _playerNumber=1;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _specialAttackCooldown;

    private Transform _transform;
    private PhysicsController _physicsController;
    private InputController _inputController = InputController.Instance();
    private Animator _animator;
    private AnimationsController _animationsController;

    private float _attackCooldownTimer;
    private float _specialAttackCooldownTimer;

    void Start()
    {
        _transform = transform;
        _physicsController = GetComponent<PhysicsController>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator, _physicsController);

        _attackCooldownTimer = _attackCooldown;
        _specialAttackCooldownTimer = _specialAttackCooldown;
    }

    private void Update()
    {
        _physicsController.InputMovement = new Vector3(_inputController.GetLeftJoystickHorizontal(_playerNumber), 0, 0);

        if (_inputController.IsAButtonPressed(_playerNumber) && _physicsController.IsGrounded())
        {
            _physicsController.Jump = true;
        }

        if(_attackCooldownTimer>_attackCooldown && _inputController.IsXButtonPressed(_playerNumber))
        {
            Attack();
            _attackCooldownTimer = 0;
        }
        _attackCooldownTimer += Time.deltaTime;

        if (_specialAttackCooldownTimer > _specialAttackCooldown && _inputController.IsYButtonPressed(_playerNumber))
        {
            SpecialAttack();
            _specialAttackCooldownTimer = 0;
        }
        _specialAttackCooldownTimer += Time.deltaTime;

        _animationsController.Update();
    }

    private void Attack()
    {
        _animationsController.Attack();
    }

    private void SpecialAttack()
    {
        _animationsController.SpecialAttack();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward);
    }

}
