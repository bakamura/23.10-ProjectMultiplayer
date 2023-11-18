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
        private IActivable[] _activableInterfaceArray;
        private const float _internalCooldown = .02f;
        private float _lastTimeUsed;
        private void Awake()
        {
            _activableInterfaceArray = new IActivable[_activablesListReference.Count];
            for (int i = 0; i < _activablesListReference.Count; i++)
            {
                _activableInterfaceArray[i] = _activablesListReference[i].GetComponent<IActivable>();
            }
        }

        public void Interact()
        {
            if(_lastTimeUsed >= _internalCooldown)
            {
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