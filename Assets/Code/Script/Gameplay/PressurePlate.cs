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
        [Networked(OnChanged = nameof(OnInteractedChanged), OnChangedTargets = OnChangedTargets.InputAuthority)] private NetworkBool _hasBeenPressed { get; set; }

        private void Awake()
        {
            _activableInterfaceArray = new IActivable[_activablesListReference.Count];
            for (int i = 0; i < _activablesListReference.Count; i++)
            {
                _activableInterfaceArray[i] = _activablesListReference[i].GetComponent<IActivable>();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Runner.IsServer && !_hasBeenPressed)
            {
                Size.Size temp = collision.gameObject.GetComponent<Size.Size>();
                if (temp && temp.Type == _sizeDesired)
                {
                    for (int i = 0; i < _activableInterfaceArray.Length; i++) _activableInterfaceArray[i].Activate();
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
                    for (int i = 0; i < _activableInterfaceArray.Length; i++) _activableInterfaceArray[i].Deactivate();
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
