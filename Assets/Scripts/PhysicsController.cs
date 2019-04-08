using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsController : MonoBehaviour
{

    [SerializeField] private float _acceleration = 0;
    [SerializeField] private float _drag = 0;
    [SerializeField] private float _maximumXZVelocity = (30 * 1000) / (60 * 60); //[m/s] 30km/h
    [SerializeField] private float _jumpHeight = 0;

    [SerializeField] private GroundColliderScript _groundCollider;
    [SerializeField] private LayerMask _mapLayerMask;

    private Transform _transform;
    private Rigidbody _rigidbody;
    private Transform _absoluteForward;

    private Vector3 _velocity = Vector3.zero; // [m/s]

    public bool Jump { get; set; }
    private bool _isJumping;
    private float _notGroundedTimer;

    public Vector3 InputMovement { get; set; } = Vector3.zero;

    void Start()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _absoluteForward = CameraScript.MainCameraTransform;
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
        Vector3 relativeMov = relativeRot * InputMovement;
        _velocity += relativeMov * _acceleration * Time.deltaTime;
    }

    private void ApplyRotation()
    {
        float movement = InputMovement.x;
        if (movement < 0)
        {
            _transform.rotation = Quaternion.LookRotation(-_absoluteForward.right);
        }
        else
        {
            _transform.rotation = Quaternion.LookRotation(_absoluteForward.right);
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
        if (IsGrounded() && Jump)
        {
            _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            Jump = false;
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
