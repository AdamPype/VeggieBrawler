using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _aliveTime;
    public PlayerScript Owner { get; set; }
    public int Damage { get; set; }
    public float CustomKnockback { get; set; }

    private void Update()
        {
        transform.parent.position += transform.parent.forward * _bulletSpeed * Time.deltaTime;

        _aliveTime -= Time.deltaTime;
        if (_aliveTime <= 0)
            Destroy(transform.root.gameObject);
        }

    private void OnTriggerEnter(Collider other)
        {
        PlayerScript hitPlayer = other.GetComponent<PlayerScript>();

        if (hitPlayer && hitPlayer != Owner)
            {
            hitPlayer.TakeDamage(Damage, transform.position, CustomKnockback);
            hitPlayer.AttackHitEffect();
            transform.GetChild(0).parent = null;
            Destroy(transform.root.gameObject);
            }
        }
    }
