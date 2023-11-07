using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory
{
    public class PressurePlate : NetworkBehaviour
    {
        [SerializeField] private Size.Size.SizeType _sizeDesired;
        [SerializeField] private IActivable[] _activablesList;
        [Networked(OnChanged = nameof(OnInteractedChanged), OnChangedTargets = OnChangedTargets.InputAuthority)] private NetworkBool _hasBeenPressed { get; set; }        

        private void OnCollisionEnter(Collision collision)
        {
            if (Runner.IsServer && !_hasBeenPressed)
            {
                Size.Size temp = collision.gameObject.GetComponent<Size.Size>();
                if (temp && temp.Type == _sizeDesired)
                {
                    for (int i = 0; i < _activablesList.Length; i++) _activablesList[i].Activate();
                    _hasBeenPressed = true;                    
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (Runner.IsServer && _hasBeenPressed)
            {
                Size.Size temp = collision.gameObject.GetComponent<Size.Size>();
                if (temp && temp.Type == _sizeDesired)
                {
                    for (int i = 0; i < _activablesList.Length; i++) _activablesList[i].Deactivate();
                    _hasBeenPressed = false;
                }
            }
        }

        private static void OnInteractedChanged(Changed<PressurePlate> changed)
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
