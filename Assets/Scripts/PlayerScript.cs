using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PhysicsController))]
public class PlayerScript : MonoBehaviour
{
    [SerializeField] [Range(1, 2)] private int _playerNumber=1;

    [Header("Attack fields")]
    [SerializeField] private int _attackDamage=0;
    [SerializeField] private float _attackCooldown=0;
    [SerializeField] private AttackCollider[] _attackColliders;
    [SerializeField] private Vector2 _attackDamageTimeRange=Vector2.zero;

    [Header("Special attack fields")]
    [SerializeField] private int _specialAttackDamage=0;
    [SerializeField] private float _specialAttackCooldown=0;
    [SerializeField] private AttackCollider[] _specialAttackColliders;
    [SerializeField] private Vector2 _specialAttackDamageTimeRange=Vector2.zero;


    private Transform _transform;
    private PhysicsController _physicsController;
    private Animator _animator;
    private AnimationsController _animationsController;

    private float _attackCooldownTimer;
    private float _specialAttackCooldownTimer;
    private Coroutine _generalAttackCoroutine;

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
        _physicsController.InputMovement = new Vector3(InputController.GetHorizontalMovement(_playerNumber), 0, 0);

        if (InputController.IsJumpButtonPressed(_playerNumber) && _physicsController.IsGrounded())
        {
            _physicsController.Jump = true;
        }

        TryAttack();
        TrySpecialAttack();

        _animationsController.Update();
    }

    private void TryAttack()
    {
        if (_attackCooldownTimer > _attackCooldown && InputController.IsAttackButtonPressed(_playerNumber))
        {
            Attack();
        }
        _attackCooldownTimer += Time.deltaTime;
    }

    private void TrySpecialAttack()
    {
        if (_specialAttackCooldownTimer > _specialAttackCooldown && InputController.IsSpecialAttackButtonPressed(_playerNumber))
        {
            SpecialAttack();
        }
        _specialAttackCooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        StartAttackCoroutine(TryAttackDamageOpponent());
        _animationsController.Attack();
        _attackCooldownTimer = 0;
    }

    private void SpecialAttack()
    {
        StartAttackCoroutine(TrySpecialAttackDamageOpponent());
        _animationsController.SpecialAttack();
        _specialAttackCooldownTimer = 0;
    }

    private void StartAttackCoroutine(IEnumerator attack)
    {
        if (_generalAttackCoroutine != null)
            StopCoroutine(_generalAttackCoroutine);

        _generalAttackCoroutine = StartCoroutine(attack);
    }

    private IEnumerator TryAttackDamageOpponent()
    {
        Debug.Log("ATTACK");
        float timer = 0;
        while (timer < _attackDamageTimeRange.x)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < _attackDamageTimeRange.y)
        {
            foreach (AttackCollider attackCollider in _attackColliders)
            {
                PlayerScript opponent = attackCollider.Opponent;
                if (opponent != null)
                {
                    opponent.TakeDamage(_attackDamage);
                    timer = _attackDamageTimeRange.y;
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator TrySpecialAttackDamageOpponent()
    {
        Debug.Log("SPECIAL ATTACK");
        float timer = 0;
        while (timer < _specialAttackDamageTimeRange.x)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < _specialAttackDamageTimeRange.y)
        {
            foreach (AttackCollider specialAttackCollider in _specialAttackColliders)
            {
                PlayerScript opponent = specialAttackCollider.Opponent;
                if (opponent != null)
                {
                    opponent.TakeDamage(_specialAttackDamage);
                    timer = _specialAttackDamageTimeRange.y;
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void TakeDamage(int damage)
    {
        if (_generalAttackCoroutine != null)
            StopCoroutine(_generalAttackCoroutine);

        Debug.Log("DAMAGE");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward);
    }

}
