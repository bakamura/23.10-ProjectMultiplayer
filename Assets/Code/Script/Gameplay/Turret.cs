using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory
{


    public class Turret : NetworkBehaviour
    {
        [SerializeField] private float _shootDelay;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Vector3 _shootPoint;
        private TickTimer _tickTimer;
        private Vector3 _initialPosition;
        private AudioSource _audioSource;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private Color _debugGizmoColor;
        [SerializeField] private float _debugGizmoSize;
#endif

        public override void Spawned()
        {
            _audioSource = GetComponent<AudioSource>();
            if (Runner.IsServer)
            {                
                _initialPosition = transform.position;
                _tickTimer = TickTimer.CreateFromSeconds(Runner, _shootDelay);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_tickTimer.Expired(Runner))
            {
                Runner.Spawn(_bulletPrefab, transform.position, Quaternion.identity, Object.InputAuthority, (runner, spawnedBullet) =>
                {
                    spawnedBullet.GetComponent<Bullet>().Shoot(_shootPoint + _initialPosition, transform.forward);
                }
                );
                _tickTimer = TickTimer.CreateFromSeconds(Runner, _shootDelay);
                Rpc_OnShoot();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_OnShoot()
        {
            UpdateVisuals();
        }

        /// <summary>
        /// This method will play any feedbacks that needs to hapen when this object changes ex: particles, materias, sounds etc
        /// </summary>
        private void UpdateVisuals()
        {
            if (_audioSource.clip) _audioSource.Play();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _debugGizmoColor;
            Gizmos.DrawSphere(transform.position + _shootPoint, _debugGizmoSize);
        }
#endif
    }
}