using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{

    [SerializeField] [Range(1, 2)] private int _playerNumber=1;

    [SerializeField] private float _acceleration = 0;
    [SerializeField] private float _drag = 0;
    [SerializeField] private float _maximumXZVelocity = (30 * 1000) / (60 * 60); //[m/s] 30km/h
    [SerializeField] private float _jumpHeight = 0;

    [SerializeField] private GroundColliderScript _groundCollider;


    private Transform _transform;
    private Rigidbody _rigidbody;
    private Transform _model;
    //private Animator _anim;
    private InputController _inputController = InputController.Instance();


    private Vector3 _velocity = Vector3.zero; // [m/s]
    private Vector3 _inputMovement;
    private bool _jump;
    private bool _isJumping;
    private float _notGroundedTimer;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _model = transform.Find("Model");
        _transform = transform;
        //_anim = GetComponent<Animator>();
        //_input = GetComponent<ControlScript>();
    }

    private void Update()
    {
        _inputMovement = _inputController.GetRightJoystickFromPlayer(_playerNumber);

        if (_inputController.IsAButtonPressed(_playerNumber) && !_isJumping)
        {
            _jump = true;
        }

        if (_velocity.magnitude > 0.1f)
            {
            _model.localEulerAngles = Vector3.Scale(Quaternion.LookRotation(_velocity).eulerAngles, Vector3.up);
            }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward);
    }

    void FixedUpdate()
    {
        ApplyGround();
        ApplyGravity();

        ApplyMovement();

        ApplyDragOnGround();
        ApplyDragInAir();
        ApplyJump();

        LimitXZVelocity();
        DoMovement();
    }


    private void ApplyGround()
    {
        if (_groundCollider.isGrounded() && _velocity.y < 0)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
            _isJumping = false;
            _notGroundedTimer = 0;
        }
    }

    private void ApplyGravity()
    {
        if (!_groundCollider.isGrounded())
        {
            _velocity += Physics.gravity * Time.deltaTime;
            _notGroundedTimer += Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        //get relative rotation from camera
        Vector3 xzForward = Vector3.Scale(_transform.forward, new Vector3(1, 0, 1));
        Quaternion relativeRot = Quaternion.LookRotation(xzForward);

        //move in relative direction
        Vector3 relativeMov = relativeRot * _inputMovement;
        _velocity += relativeMov * _acceleration * Time.deltaTime;
    }

    private void LimitXZVelocity()
    {
        Vector3 yVel = Vector3.Scale(_velocity, Vector3.up);
        Vector3 xzVel = Vector3.Scale(_velocity, new Vector3(1, 0, 1));

        xzVel = Vector3.ClampMagnitude(xzVel, _maximumXZVelocity);

        _velocity = xzVel + yVel;
    }

    private void ApplyDragOnGround()
    {
        if (_groundCollider.isGrounded())
        {
            //drag
            _velocity = _velocity * (1 - _drag * Time.deltaTime); //same as lerp
        }
    }

    private void ApplyDragInAir()
    {
        if (!_groundCollider.isGrounded())
        {
            float y = _velocity.y;
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, 0.06f);
            _velocity.y = y;
        }
    }

    private void ApplyJump()
    {
        if (_groundCollider.isGrounded() && _jump)
        {
            _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            _jump = false;
            _isJumping = true;
        }
    }

    private void DoMovement()
    {
        _rigidbody.velocity = _velocity;
    }
}
