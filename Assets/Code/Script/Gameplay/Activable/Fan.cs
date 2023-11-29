using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace ProjectMultiplayer.ObjectCategory
{
    public class Fan : NetworkBehaviour, IActivable
    {
        [SerializeField] private SpeedValueData[] _speedValues;
        [SerializeField] private LayerMask _objectsAffectedLayer;
        [SerializeField, Min(0f)] private float _fanRotationSpeed;
        [SerializeField] private Transform _objectToRotate;
        [SerializeField] private bool _turnCounterClokwise;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool _debugActive;
        [SerializeField] private Color _debugColor;
#endif

        private bool _hasBeenActivated { get; set; }

        private Dictionary<int, MovableObjectData> _objectsInsideArea = new Dictionary<int, MovableObjectData>();
        private Coroutine _moveObjectsCoroutine = null;
        private Coroutine _fanRotationCoroutine = null;
        private AudioSource _audioSource;
        private WaitForSeconds _delay;

        [Serializable]
        private struct SpeedValueData
        {
            public Size.Size.SizeType SizeType;
            public float Speed;
        }

        [Serializable]
        private struct MovableObjectData
        {
            public Size.Size.SizeType SizeType;
            public NetworkRigidbody Rigidbody;

            public MovableObjectData(Size.Size.SizeType sizeType, NetworkRigidbody rb)
            {
                SizeType = sizeType;
                Rigidbody = rb;
            }
        }

        public override void Spawned()
        {
            _delay = new WaitForSeconds(Runner.DeltaTime);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();            
        }

        private void OnTriggerEnter(Collider other)
        {
            Size.Size temp = other.GetComponent<Size.Size>();
            if (Runner.IsServer && _hasBeenActivated && temp && ContainsSpeedDataForSizeType(temp.Type) && !_objectsInsideArea.ContainsKey(temp.gameObject.GetHashCode()))
            {
                _objectsInsideArea.Add(temp.gameObject.GetHashCode(), new MovableObjectData(temp.Type, temp.GetComponent<NetworkRigidbody>()));
                if (_moveObjectsCoroutine == null) _moveObjectsCoroutine = StartCoroutine(MoveObjects());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Size.Size temp = other.GetComponent<Size.Size>();
            if (Runner.IsServer && _hasBeenActivated && temp && ContainsSpeedDataForSizeType(temp.Type) && _objectsInsideArea.ContainsKey(temp.gameObject.GetHashCode()))
            {
                _objectsInsideArea.Remove(temp.gameObject.GetHashCode());
            }
        }

        private IEnumerator MoveObjects()
        {
            MovableObjectData[] objectsInThisFrame;
            while (_objectsInsideArea.Count > 0)
            {
                objectsInThisFrame = new MovableObjectData[_objectsInsideArea.Values.Count];
                _objectsInsideArea.Values.ToArray().CopyTo(objectsInThisFrame, 0);
                for (int i = 0; i < objectsInThisFrame.Length; i++)
                {
                    RaycastHit[] hits = Physics.RaycastAll(objectsInThisFrame[i].Rigidbody.transform.position, -transform.forward, Vector3.Distance(objectsInThisFrame[i].Rigidbody.transform.position, transform.position), _objectsAffectedLayer);
                    if (hits == null || !CheckForBlockingCollisions(hits, objectsInThisFrame[i].SizeType))
                        objectsInThisFrame[i].Rigidbody.Rigidbody.AddForce(transform.forward * GetSpeedData(objectsInThisFrame[i].SizeType).Speed, ForceMode.Force);
                }
                yield return _delay;
            }

            _moveObjectsCoroutine = null;
        }

        private bool CheckForBlockingCollisions(RaycastHit[] hits, Size.Size.SizeType typeToCompare)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponent<Size.Size>().Type >= typeToCompare) return true;
            }
            return false;
        }

        private SpeedValueData GetSpeedData(Size.Size.SizeType typeToCompare)
        {
            for (int i = 0; i < _speedValues.Length; i++)
            {
                if (_speedValues[i].SizeType == typeToCompare)
                {
                    return _speedValues[i];
                }
            }
            return new SpeedValueData();
        }

        private bool ContainsSpeedDataForSizeType(Size.Size.SizeType typeToCompare)
        {
            for (int i = 0; i < _speedValues.Length; i++)
            {
                if (_speedValues[i].SizeType == typeToCompare)
                {
                    return true;
                }
            }
            return false;
        }

        public void Activate()
        {
            Rpc_OnInteractedChanged(true);
        }

        public void Deactivate()
        {
            Rpc_OnInteractedChanged(false);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_OnInteractedChanged(bool hasBeenActivated)
        {
            _hasBeenActivated = hasBeenActivated;
            UpdateVisuals();
        }

        /// <summary>
        /// This method will play any feedbacks that needs to hapen when this object changes ex: particles, materias, sounds etc
        /// </summary>
        private void UpdateVisuals()
        {
            if (_hasBeenActivated)
            {
                if (_audioSource.clip) _audioSource.Play();
                if(_fanRotationCoroutine == null)_fanRotationCoroutine = StartCoroutine(FanRotation());
            }
            else
            {
                if (_fanRotationCoroutine != null)
                {
                    if (_audioSource.clip) _audioSource.Stop();
                    StopCoroutine(_fanRotationCoroutine);
                    _fanRotationCoroutine = null;
                }
            }
        }

        private IEnumerator FanRotation()
        {
            WaitForFixedUpdate delay = new WaitForFixedUpdate();
            while (true)
            {
                _objectToRotate.Rotate(Vector3.right, _turnCounterClokwise ? -_fanRotationSpeed * Time.fixedDeltaTime : _fanRotationSpeed * Time.fixedDeltaTime);
                yield return delay;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_debugActive)
            {
                Gizmos.color = _debugColor;
                Gizmos.matrix = transform.localToWorldMatrix;
                BoxCollider boxCollider = GetComponent<BoxCollider>();
                Vector3 size = new Vector3(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y, boxCollider.size.z * transform.localScale.z);
                Gizmos.DrawCube(boxCollider.center, size);
            }
        }
#endif
    }
}