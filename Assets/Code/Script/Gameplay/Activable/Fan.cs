using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{
    public class Fan : NetworkBehaviour, IActivable
    {
        [SerializeField] private SpeedValueData[] _speedValues;
        [Networked(OnChanged = nameof(OnInteractedChanged), OnChangedTargets = OnChangedTargets.InputAuthority)] private NetworkBool _hasBeenActivated { get; set; }

        [System.Serializable]
        private struct SpeedValueData
        {
            public Size.Size.SizeType SizeType;
            public float Speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Runner.IsServer && _hasBeenActivated)
            {
                //Runner.LagCompensation.Raycast();
            }
        }

        public void Activate()
        {
            _hasBeenActivated = true;
        }

        public void Deactivate()
        {
            _hasBeenActivated = false;
        }

        private static void OnInteractedChanged(Changed<Fan> changed)
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