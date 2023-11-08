using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.Player;

public class Bullet : NetworkBehaviour {
    [SerializeField] private float _movementSpeed;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Shoot(Vector3 positionInitial, Vector3 direction) {
        transform.position = positionInitial;
        _rigidbody.velocity = direction * _movementSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<Player>()?.TryDamage();
        Runner.Despawn(Object);
    }

}
