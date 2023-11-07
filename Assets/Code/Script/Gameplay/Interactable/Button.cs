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
        private bool _hasBeenPressed;
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

    }
}