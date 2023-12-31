using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.Player;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private AudioClip _hitSound;
    private NetworkRigidbody _networkRb;

    private void Awake()
    {
        _networkRb = GetComponent<NetworkRigidbody>();
    }

    public void Shoot(Vector3 positionInitial, Vector3 direction)
    {
        transform.position = positionInitial;
        _networkRb.Rigidbody.velocity = direction * _movementSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Runner.IsServer)
        {
            //if(collision.gameObject.GetHashCode() != gameObject.GetHashCode())
            //{
            if(_hitSound) AudioSource.PlayClipAtPoint(_hitSound, transform.position);
            Debug.Log(collision.gameObject.name);
            collision.gameObject.GetComponent<Player>()?.TryDamage();
            Runner.Despawn(Object);
            //}
        }
    }

}
