using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{    
    public class Button : NetworkBehaviour, IInteractable, IActivable
    {
        [SerializeField] private List<GameObject> _activablesListReference = new List<GameObject>();
        [SerializeField] private Transform _movablePart;
        [SerializeField] private float _buttonDistance;
        private AudioSource _audioSource;
        private Vector3 _baseMovablepartPosition;
        private IActivable[] _activableInterfaceArray;
        private bool _hasBeenPressed;
        private void Awake()
        {
            _activableInterfaceArray = new IActivable[_activablesListReference.Count];
            _baseMovablepartPosition = _movablePart.localPosition;
            _audioSource = GetComponent<AudioSource>();
            for (int i = 0; i < _activablesListReference.Count; i++)
            {
                _activableInterfaceArray[i] = _activablesListReference[i].GetComponent<IActivable>();
            }
        }

        public void Interact()
        {
            if (!_hasBeenPressed)
            {
                for(int i = 0; i < _activableInterfaceArray.Length; i++)
                {
                    _activableInterfaceArray[i].Activate();
                }
                Rpc_OnInteractedChanged(true);
            }
        }

        public void Activate()
        {
            if (_hasBeenPressed)
            {
                for (int i = 0; i < _activableInterfaceArray.Length; i++)
                {
                    _activableInterfaceArray[i].Deactivate();
                }
                Rpc_OnInteractedChanged(false);
            }
        }

        public void Deactivate()
        {

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
            if(_activablesListReference != null)
            {
                for(int i = 0; i < _activablesListReference.Count; i++)
                {
                    if (_activablesListReference[i] && _activablesListReference[i].GetComponent<IActivable>() == null) 
                        _activablesListReference.Remove(_activablesListReference[i]);
                }
            }
        }

    }
}