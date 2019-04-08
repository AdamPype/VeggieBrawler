using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PhysicsController))]
public class PlayerScript : MonoBehaviour
{

    [SerializeField] [Range(1, 2)] private int _playerNumber=1;

    private Transform _transform;
    private PhysicsController _physicsController;
    private InputController _inputController = InputController.Instance();
    private Animator _animator;
    private AnimationsController _animationsController;

    void Start()
    {
        _transform = transform;
        _physicsController = GetComponent<PhysicsController>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator, _physicsController);
    }

    private void Update()
    {
        _physicsController.InputMovement = new Vector3(_inputController.GetLeftJoystickHorizontal(_playerNumber), 0, 0);

        if (_inputController.IsAButtonPressed(_playerNumber) && _physicsController.IsGrounded())
        {
            _physicsController.Jump = true;
        }

        _animationsController.Update();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward);
    }

}
