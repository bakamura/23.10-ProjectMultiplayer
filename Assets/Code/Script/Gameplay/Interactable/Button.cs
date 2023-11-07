using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{    
    public class Button : NetworkBehaviour, IInteractable, IActivable
    {
        [SerializeField] private IActivable[] _activablesList;
        [Networked(OnChanged = nameof(OnInteractedChanged), OnChangedTargets = OnChangedTargets.InputAuthority)] private NetworkBool _hasBeenPressed { get; set; }
        public void Interact()
        {
            if (!_hasBeenPressed)
            {
                for(int i = 0; i < _activablesList.Length; i++)
                {
                    _activablesList[i].Activate();
                }
                _hasBeenPressed = true;
            }
        }

        public void Activate()
        {
            if (_hasBeenPressed)
            {
                for (int i = 0; i < _activablesList.Length; i++)
                {
                    _activablesList[i].Deactivate();
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

    }
}