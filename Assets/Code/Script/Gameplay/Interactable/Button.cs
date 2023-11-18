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
        private IActivable[] _activableInterfaceArray;
        [Networked(OnChanged = nameof(OnInteractedChanged), OnChangedTargets = OnChangedTargets.All)] private NetworkBool _hasBeenPressed { get; set; }
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
            if (!_hasBeenPressed)
            {
                for(int i = 0; i < _activableInterfaceArray.Length; i++)
                {
                    _activableInterfaceArray[i].Activate();
                }
                _hasBeenPressed = true;
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
                _hasBeenPressed = false;
            }
        }

        public void Deactivate()
        {

        }

        private static void OnInteractedChanged(Changed<Button> changed)
        {
            changed.Behaviour.UpdateVisuals(changed.Behaviour._hasBeenPressed);
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