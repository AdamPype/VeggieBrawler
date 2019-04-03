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
    [SerializeField] private LayerMask _mapLayerMask;
    

    private Transform _transform;
    private Rigidbody _rigidbody;
    private InputController _inputController = InputController.Instance();
    private Animator _animator;
    private AnimationsController _animationsController;
    private Transform _absoluteForward;


    private Vector3 _velocity = Vector3.zero; // [m/s]
    public Vector3 _inputMovement = Vector3.zero;
    public Vector3 InputMovement { get =>_inputMovement; }

    private bool _jump;
    private bool _isJumping;
    private float _notGroundedTimer;

    void Start()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator, this);
        _absoluteForward = CameraScript.MainCameraTransform;
    }

    private void Update()
    {
        _inputMovement = new Vector3(_inputController.GetLeftJoystickHorizontal(_playerNumber),0,0);
        //_inputMovement.z = 0;

        if (_inputController.IsAButtonPressed(_playerNumber) && !_isJumping)
        {
            _jump = true;
        }

        _animationsController.Update();
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
        ApplyRotation();

        ApplyDragOnGround();
        ApplyDragInAir();
        ApplyJump();

        LimitXZVelocity();
        DoMovement();
    }


    private void ApplyGround()
    {
        if (IsGrounded() && _velocity.y < 0)
        {
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
            _isJumping = false;
            _notGroundedTimer = 0;
        }
    }

    private void ApplyGravity()
    {
        if (!IsGrounded())
        {
            _velocity += Physics.gravity * Time.deltaTime;
            _notGroundedTimer += Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        //get relative rotation from camera
        Vector3 xzForward = Vector3.Scale(_absoluteForward.forward, new Vector3(1, 0, 1));
        Quaternion relativeRot = Quaternion.LookRotation(xzForward);

        //move in relative direction
        Vector3 relativeMov = relativeRot * _inputMovement;
        _velocity += relativeMov * _acceleration * Time.deltaTime;
    }

    private void ApplyRotation()
    {
        float movement = _inputController.GetLeftJoystickHorizontal(_playerNumber);
        if (movement < 0)
        {
            _transform.eulerAngles = new Vector3(0, -90, 0);
        }
        else
        {
            _transform.eulerAngles = new Vector3(0, 90, 0);
        }

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
        if (IsGrounded())
        {
            //drag
            _velocity = _velocity * (1 - _drag * Time.deltaTime); //same as lerp
        }
    }

    private void ApplyDragInAir()
    {
        if (!IsGrounded())
        {
            float y = _velocity.y;
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, 0.1f);
            _velocity.y = y;
        }
    }

    private void ApplyJump()
    {
        if (IsGrounded() && _jump)
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

    public bool IsGrounded()
    {
        return _groundCollider.isGrounded();
    }

    public float GetDistanceFromGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(_transform.position, Vector3.down, out hit, 1000, _mapLayerMask))
        {
            return (hit.point - _transform.position).magnitude;
        }
        return Mathf.Infinity;
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody.velocity;
    }
}
