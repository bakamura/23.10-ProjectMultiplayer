using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory
{
    public class PressurePlate : NetworkBehaviour
    {
        [SerializeField] private List<GameObject> _activablesListReference = new List<GameObject>();
        private IActivable[] _activableInterfaceArray;
        [SerializeField] private Size.Size.SizeType _sizeDesired;
        [SerializeField] private Transform _movablePart;
        [SerializeField] private float _buttonDistance;
        private AudioSource _audioSource;
        private Vector3 _baseMovablepartPosition;
        private bool _hasBeenPressed;

        private void Awake()
        {
            _activableInterfaceArray = new IActivable[_activablesListReference.Count];
            _baseMovablepartPosition = _movablePart.localPosition;
            for (int i = 0; i < _activablesListReference.Count; i++)
            {
                _activableInterfaceArray[i] = _activablesListReference[i].GetComponent<IActivable>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Runner.IsServer && !_hasBeenPressed)
            {
                Size.Size temp = other.GetComponent<Size.Size>();
                if (temp && temp.Type == _sizeDesired)
                {
                    for (int i = 0; i < _activableInterfaceArray.Length; i++) _activableInterfaceArray[i].Activate();
                    Rpc_OnInteractedChanged(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (Runner.IsServer && _hasBeenPressed)
            {
                Size.Size temp = other.GetComponent<Size.Size>();
                if (temp && temp.Type == _sizeDesired)
                {
                    for (int i = 0; i < _activableInterfaceArray.Length; i++) _activableInterfaceArray[i].Deactivate();
                    Rpc_OnInteractedChanged(false);
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_OnInteractedChanged(bool hasBeenActivated)
        {
            _hasBeenPressed = hasBeenActivated;
            UpdateVisuals();
        }

        /// <summary>
        /// This method will play any feedbacks that needs to hapen when this object changes ex: particles, materias, sounds etc
        /// </summary>
        private void UpdateVisuals()
        {
            _movablePart.localPosition = _hasBeenPressed ? _movablePart.localPosition + transform.up * _buttonDistance : _baseMovablepartPosition;
            if (_audioSource.clip) _audioSource.Play();
        }

        private void OnValidate()
        {
            if (_activablesListReference != null)
            {
                for (int i = 0; i < _activablesListReference.Count; i++)
                {
                    if (_activablesListReference[i] && _activablesListReference[i].GetComponent<IActivable>() == null)
                        _activablesListReference.Remove(_activablesListReference[i]);
                }
            }
        }
    }
}
