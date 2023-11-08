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
        [Networked(OnChanged = nameof(OnActivateChanged), OnChangedTargets = OnChangedTargets.InputAuthority)] private NetworkBool _hasBeenActivated { get; set; }

        private Dictionary<int, MovableObjectData> _objectsInsideArea = new Dictionary<int, MovableObjectData>();
        private Coroutine _moveObjectsCoroutine = null;
        private WaitForSeconds _delay = new WaitForSeconds(_tickDelay);
        private const float _tickDelay = .2f;

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
            _hasBeenActivated = true;
        }

        public void Deactivate()
        {
            _hasBeenActivated = false;
        }

        private static void OnActivateChanged(Changed<Fan> changed)
        {
            changed.Behaviour.UpdateVisuals(changed.Behaviour._hasBeenActivated);
        }

        /// <summary>
        /// This method will play any feedbacks that needs to hapen when this object changes ex: particles, materias, sounds etc
        /// </summary>
        /// <param name="isActive"></param>
        private void UpdateVisuals(bool isActive)
        {
            //TODO SEE WHAT WILL CHANGE IN VISUAL
            transform.localScale = isActive ? Vector3.one : Vector3.one / 2;
        }
    }
}