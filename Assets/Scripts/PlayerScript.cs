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
    [SerializeField] private float _flinchTimeReceivedModifier = 0.5f;
    [SerializeField] private float _knockbackReceivedModifier = 0;
    [SerializeField] private bool _canControlDuringAttack = false;

    [Header("Attack fields")]
    [SerializeField] private int _attackDamage=0;
    [SerializeField] private AttackCollider[] _attackColliders;
    public float AttackDuration = 0;
    [SerializeField] private Vector2 _attackDamageTimeRange=Vector2.zero;

    [SerializeField] private float _customKnockbackAttack = -1;
    [SerializeField] private float _customFlinchAttack = -1;
    [SerializeField] private bool _useAttackMotion=false;
    [SerializeField] private float _freezeAttack = 0.1f;
    [SerializeField] private float _shakeAttack = 0.04f;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private float _bulletDelayTime;

    [Header("Special attack fields")]
    [SerializeField] private int _specialAttackDamage=0;
    [SerializeField] private AttackCollider[] _specialAttackColliders;
    public float SpecialAttackDuration = 0;
    [SerializeField] private Vector2 _specialAttackDamageTimeRange=Vector2.zero;
    [SerializeField] private float _customKnockbackSpecialAttack = -1;
    [SerializeField] private float _customFlinchSpecialAttack = -1;
    [SerializeField] private bool _useSpecialAttackMotion=false;
    [SerializeField] private float _freezeSpecial = 0.3f;
    [SerializeField] private float _shakeSpecial = 0.2f;

    public int MaxHealth { get => _maxHealth; }
    public int Health { get; private set; }
    public int PlayerNumber { get => _playerNumber; set => _playerNumber=value; }

    private Transform _transform;
    private PhysicsController _physicsController;
    private Animator _animator;
    private AnimationsController _animationsController;

    public float AttackCooldownTimer { get; set; }
    public float SpecialAttackCooldownTimer { get; set; }
    public bool SpecialAttacking { get; private set; }

    private Coroutine _generalAttackCoroutine;
    //private float _flinchTimer = 0;
    private bool _isFlinched=false;
    private bool _isDead;
    private float _bulletDelayTimer;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        _transform = transform;
        _physicsController = GetComponent<PhysicsController>();
        _animator = GetComponent<Animator>();
        _animationsController = new AnimationsController(_animator, _physicsController);

        Health = _maxHealth;
        AttackCooldownTimer = AttackDuration;
        SpecialAttackCooldownTimer = SpecialAttackDuration;
        //_flinchTimer = _flinchTime;
    }

    private void Update()
    {
        if (_isDead)
        {
            GameControllerScript.Instance.EndGame(_playerNumber);
            return;
        }


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
        if (_generalAttackCoroutine==null && InputController.IsAttackButtonPressed(_playerNumber))
        {
            Attack();
        }
        AttackCooldownTimer += Time.deltaTime;
    }

    private void TrySpecialAttack()
    {
        if (_generalAttackCoroutine == null && InputController.IsSpecialAttackButtonPressed(_playerNumber))
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

        bool hasLandedHit = false;

        while (timer < _attackDamageTimeRange.y)
        {
            if (!_bullet)
                {
                //regular attack
                if (!hasLandedHit)
                    {
                    foreach (AttackCollider attackCollider in _attackColliders)
                        {
                        PlayerScript opponent = attackCollider.Opponent;
                        if (opponent != null)
                            {
                            opponent.TakeDamage(_attackDamage, attackCollider.HitOrigin, _customKnockbackAttack, _customFlinchAttack);
                            hasLandedHit = true;
                            AttackHitEffect();
                            break;
                            }
                        }
                    }
                }
            else if (_bulletDelayTimer <= 0)
                {
                _bulletDelayTimer = _bulletDelayTime;
                BulletScript bullet = Instantiate(_bullet).transform.Find("Col").GetComponent<BulletScript>();
                bullet.transform.parent.position = _bulletSpawn.position;
                bullet.transform.parent.rotation = _bulletSpawn.rotation;
                bullet.Damage = _attackDamage;
                bullet.Owner = this;
                bullet.CustomKnockback = _customKnockbackAttack;
                }

            _bulletDelayTimer -= Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        _bulletDelayTimer = 0;

        while (timer < AttackDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _generalAttackCoroutine = null;
        Debug.Log("finished");
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

        bool hasLandedHit = false;

        while (timer < _specialAttackDamageTimeRange.y)
        {
            if (!hasLandedHit)
            {
                foreach (AttackCollider specialAttackCollider in _specialAttackColliders)
                {
                    PlayerScript opponent = specialAttackCollider.Opponent;
                    if (opponent != null)
                        {
                        opponent.TakeDamage(_specialAttackDamage, specialAttackCollider.HitOrigin, _customKnockbackSpecialAttack, _customFlinchSpecialAttack);
                        hasLandedHit = true;
                        SpecialAttackHitEffect();
                        break;
                        }
                    }
            }
            SpecialAttacking = true;

            timer += Time.deltaTime;
            yield return null;
        }
        SpecialAttacking = false;


        while (timer < SpecialAttackDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _generalAttackCoroutine = null;
        UseAnimationMotion(false);
    }

    private void SpecialAttackHitEffect()
        {
        FixedTime.FreezeTime(_freezeSpecial);
        CameraScript.Shake(_shakeAttack);
        }

    public void TakeDamage(int damage, Vector3 origin, float customKnockback = -1, float customFlinch = -1)
    {
        if (_generalAttackCoroutine != null)
        {
            UseAnimationMotion(false);
            StopCoroutine(_generalAttackCoroutine);
            _generalAttackCoroutine = null;
        }
        

        float flinch = _flinchTimeReceivedModifier;
        if (customFlinch != -1) flinch += customFlinch;
        if (flinch < 0) flinch = 0;
        StartCoroutine(Flinch(flinch));

        //knockback
        float knockback = _knockbackReceivedModifier;
        if (customKnockback != -1) knockback += customKnockback;
        if (knockback < 0) knockback = 0;
        _physicsController.TakeKnockBack(knockback , origin);

        _animationsController.TakeDamage();
        Health -= damage;
        Die();
        Debug.Log("DAMAGE");
    }

    private IEnumerator Flinch(float time)
    {
        _isFlinched = true;
        yield return new WaitForSeconds(time);

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

    public void AttackHitEffect()
        {
        FixedTime.FreezeTime(_freezeAttack);
        CameraScript.Shake(_shakeAttack);
        }

}
