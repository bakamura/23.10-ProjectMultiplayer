using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{
    public class Piston : NetworkBehaviour, IActivable
    {
        [SerializeField] private Transform _movablePart;
        [SerializeField] private Transform _middlePart;
        [SerializeField] private float _middleSizePartScaleMultiplier = 3;
        [SerializeField, Tooltip("Thte travel distance is always relative to the current position")] private PistonPositionData[] _positionsData;
        [SerializeField] private float _speed;
        //private bool _isMoving;
        //private bool _isActive;
        private sbyte _directionSign;
        private sbyte _currentActivations;
        private PistonPositionData _currentPosition;
        private AudioSource _audioSource;
        private Coroutine _pistonMovmentCoroutine = null;

        [System.Serializable]
        private struct PistonPositionData
        {
            [Min(1f)] public sbyte ActivationsRequired;
            [Min(0f)] public float TravelDistance;
        }

#if UNITY_EDITOR
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private Color _debugColor;
#endif

        private void Awake()
        {
            _audioSource = GetComponentInChildren<AudioSource>();
        }
        [ContextMenu("Activate")]
        public void Activate()
        {
            if (_pistonMovmentCoroutine == null /*&& !_isActive*/)
            {
                _directionSign = 1;
                _currentActivations = (sbyte)(_currentActivations < 0 ? 1 : _currentActivations + 1);
                if (GetPositionData()) Rpc_OnInteractedChanged(_currentPosition.TravelDistance, _directionSign);
            }
        }
        [ContextMenu("Deactivate")]
        public void Deactivate()
        {
            if (_pistonMovmentCoroutine == null /*&& _isActive*/)
            {
                _directionSign = -1;
                _currentActivations--;
                if (GetPositionData()) Rpc_OnInteractedChanged(_currentPosition.TravelDistance, _directionSign);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_OnInteractedChanged(float distance, float direction)
        {
            UpdateVisuals(distance, direction);
        }

        private void UpdateVisuals(float distance, float direction)
        {
            if (_audioSource.clip) _audioSource.Play();
            StartCoroutine(MovePiston(distance, direction));
        }

        private IEnumerator MovePiston(float distance, float direction)
        {
            WaitForFixedUpdate delay = new WaitForFixedUpdate();
            Vector3 initialPos = _movablePart.position;
            while (Vector3.Distance(_movablePart.position, initialPos) < distance)
            {
                _movablePart.position += _speed * direction * Time.fixedDeltaTime * transform.up;
                _middlePart.localScale += _middleSizePartScaleMultiplier * _speed * direction * Time.fixedDeltaTime * transform.up;
                yield return delay;
            }
            if (_audioSource.clip) _audioSource.Stop();
            _pistonMovmentCoroutine = null;
        }

        //public override void FixedUpdateNetwork()
        //{
        //    if (_isMoving)
        //    {
        //        _movablePart.position += _speed * _directionSign * Runner.DeltaTime * transform.up;
        //        _middlePart.localScale += _speed * _directionSign * Runner.DeltaTime * transform.up;
        //        if (Vector3.Distance(_movablePart.position, _initialPosition) >= _currentPosition.TravelDistance)
        //        {
        //            //_isActive = _directionSign > 0;
        //            if (_audioSource.clip) _audioSource.Stop();
        //            _isMoving = false;
        //        }
        //    }
        //}

        private bool GetPositionData()
        {
            if (_directionSign > 0)
            {
                for (int i = 0; i < _positionsData.Length; i++)
                {
                    if (_positionsData[i].ActivationsRequired == _currentActivations)
                    {
                        _currentPosition = _positionsData[i];
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _positionsData.Length; i++)
                {
                    if (_positionsData[i].ActivationsRequired == _currentActivations + 1)
                    {
                        _currentPosition = _positionsData[i];
                        return true;
                    }
                }
            }
            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_drawGizmos && _positionsData != null)
            {
                Gizmos.color = _debugColor;
                Vector3 finalPos = _movablePart.position;
                for(int i = 0; i < _positionsData.Length; i++)
                {
                    finalPos += _positionsData[i].TravelDistance * transform.up;
                    Gizmos.DrawSphere(finalPos, .1f);
                    UnityEditor.Handles.Label(finalPos+ Vector3.up * .2f, i.ToString());
                }
            }
        }
#endif
    }
}