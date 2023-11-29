using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{
    public class Lever : NetworkBehaviour, IInteractable
    {
        [SerializeField] private List<GameObject> _activablesListReference = new List<GameObject>();
        [SerializeField] private Transform _movablePart;
        [SerializeField] private float _leverAngle;
        private IActivable[] _activableInterfaceArray;
        private const float _internalCooldown = 1f;
        private float _lastTimeUsed;
        private AudioSource _audioSource;
        private float _baseLeverRotation;
        private WaitForSeconds _delay;
        private void Awake()
        {
            _activableInterfaceArray = new IActivable[_activablesListReference.Count];
            _audioSource = GetComponent<AudioSource>();
            _baseLeverRotation = _movablePart.localEulerAngles.x;
            for (int i = 0; i < _activablesListReference.Count; i++)
            {
                _activableInterfaceArray[i] = _activablesListReference[i].GetComponent<IActivable>();
            }
        }

        public override void Spawned()
        {
            _delay = new WaitForSeconds(Runner.DeltaTime);
        }


        public void Interact()
        {
            if (_lastTimeUsed >= _internalCooldown)
            {
                Rpc_OnInteractedChanged();
                for (int i = 0; i < _activableInterfaceArray.Length; i++)
                {
                    _activableInterfaceArray[i].Activate();
                }
                _lastTimeUsed = 0;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_lastTimeUsed < _internalCooldown) _lastTimeUsed += Runner.DeltaTime;
        }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_OnInteractedChanged()
        {
            UpdateVisuals();
        }

        /// <summary>
        /// This method will play any feedbacks that needs to hapen when this object changes ex: particles, materias, sounds etc
        /// </summary>
        private void UpdateVisuals()
        {
            StartCoroutine(LeverAnimation());
            if (_audioSource.clip) _audioSource.Play();
        }

        IEnumerator LeverAnimation()
        {
            float step = 0;
            float initialRotation = _movablePart.localEulerAngles.x;
            //WaitForSeconds delay = new WaitForSeconds(Runner.DeltaTime);
            float finalRotation = _movablePart.localEulerAngles.x == _baseLeverRotation ? _leverAngle : _baseLeverRotation;
            while (step < 1)
            {
                step += Runner.DeltaTime / _internalCooldown;
                float val = Mathf.LerpAngle(initialRotation, finalRotation, step);
                _movablePart.localEulerAngles = new Vector3(val, _movablePart.localEulerAngles.y, _movablePart.localEulerAngles.z);
                yield return _delay;
            }
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