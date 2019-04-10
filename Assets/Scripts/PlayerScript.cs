using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PhysicsController))]
public class PlayerScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] [Range(1, 2)] private int _playerNumber=1;
    [SerializeField] private int _maxHealth = 0;
    [SerializeField] private float _flinchTime=0;
    [SerializeField] private float _knockbackForce = 10;
    [SerializeField] private bool _canControlDuringAttack = false;

    [Header("Attack fields")]
    [SerializeField] private int _attackDamage=0;
    public float AttackCooldown=0;
    [SerializeField] private AttackCollider[] _attackColliders;
    [SerializeField] private Vector2 _attackDamageTimeRange=Vector2.zero;
    [SerializeField] private bool _useAttackMotion=false;

    [Header("Special attack fields")]
    [SerializeField] private int _specialAttackDamage=0;
    public float SpecialAttackCooldown=0;
    [SerializeField] private AttackCollider[] _specialAttackColliders;
    [SerializeField] private Vector2 _specialAttackDamageTimeRange=Vector2.zero;
    [SerializeField] private bool _useSpecialAttackMotion=false;

    public int MaxHealth { get => _maxHealth; }
    public int Health { get; private set; }
    public int PlayerNumber { get => _playerNumber; }

    private Transform _transform;
    private PhysicsController _physicsController;
    private Animator _animator;
    private AnimationsController _animationsController;

    public float AttackCooldownTimer { get; set; }
    public float SpecialAttackCooldownTimer { get; set; }
    private Coroutine _generalAttackCoroutine;
    //private float _flinchTimer = 0;
    private bool _isFlinched=false;
    private bool _isDead;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        _transform = transform;
        _physicsController = GetComponent<PhysicsController>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator, _physicsController);

        Health = _maxHealth;
        AttackCooldownTimer = AttackCooldown;
        SpecialAttackCooldownTimer = SpecialAttackCooldown;
        //_flinchTimer = _flinchTime;
    }

    private void Update()
    {
        if (_isDead) return;


        if (!_isFlinched)
        {
            if(_generalAttackCoroutine == null || _canControlDuringAttack)
            {
                _physicsController.InputMovement = new Vector3(InputController.GetHorizontalMovement(_playerNumber), 0, 0);

                if (InputController.IsJumpButtonPressed(_playerNumber) && _physicsController.IsGrounded())
                {
                    _physicsController.Jump = true;
                }

                TryAttack();
                TrySpecialAttack();
            }
            else
            {
                _physicsController.InputMovement = Vector3.zero;
            }

        }
        else
        {
            _physicsController.InputMovement = Vector3.zero;
        }
        //_flinchTimer += Time.deltaTime;

        _animationsController.Update();
    }

    private void TryAttack()
    {
        if (AttackCooldownTimer > AttackCooldown && InputController.IsAttackButtonPressed(_playerNumber))
        {
            Attack();
        }
        AttackCooldownTimer += Time.deltaTime;
    }

    private void TrySpecialAttack()
    {
        if (SpecialAttackCooldownTimer > SpecialAttackCooldown && InputController.IsSpecialAttackButtonPressed(_playerNumber))
        {
            SpecialAttack();
        }
        SpecialAttackCooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        StartAttackCoroutine(TryAttackDamageOpponent());

        UseAnimationMotion(_useAttackMotion);
        _animationsController.Attack();
        AttackCooldownTimer = 0;
    }

    private void SpecialAttack()
    {
        StartAttackCoroutine(TrySpecialAttackDamageOpponent());

        UseAnimationMotion(_useSpecialAttackMotion);
        _animationsController.SpecialAttack();
        SpecialAttackCooldownTimer = 0;
    }

    private void UseAnimationMotion(bool useMotion)
    {
        if (!useMotion && _physicsController.IsKinematic)
            _physicsController.Velocity = Vector3.zero;

        _physicsController.IsKinematic = useMotion;
        _animationsController.ApplyRootMotion(useMotion);

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
                    opponent.TakeDamage(_attackDamage, attackCollider.HitOrigin);
                    timer = _attackDamageTimeRange.y;
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _generalAttackCoroutine = null;
        UseAnimationMotion(false);
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
                    opponent.TakeDamage(_specialAttackDamage, specialAttackCollider.HitOrigin);
                    timer = _specialAttackDamageTimeRange.y;
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _generalAttackCoroutine = null;
        UseAnimationMotion(false);
    }

    public void TakeDamage(int damage, Vector3 origin)
    {
        if (_generalAttackCoroutine != null)
        {
            UseAnimationMotion(false);
            StopCoroutine(_generalAttackCoroutine);
        }

        //_flinchTimer = 0;
        StartCoroutine(Flinch());
        _physicsController.TakeKnockBack(_knockbackForce, origin);
        _animationsController.TakeDamage();
        Health -= damage;
        Die();
        Debug.Log("DAMAGE");
    }

    private IEnumerator Flinch()
    {
        _isFlinched = true;
        yield return new WaitForSeconds(_flinchTime);

        _isFlinched = false;
        _animationsController.Recover();
    }

    private void Die()
    {
        if (Health <= 0)
        {
            _isDead = true;
            gameObject.layer = LayerMask.NameToLayer("NoCollisionWithPlayer");
            _animationsController.Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + Vector3.up, transform.forward);
    }

}
