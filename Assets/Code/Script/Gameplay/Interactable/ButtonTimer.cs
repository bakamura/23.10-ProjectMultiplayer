using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{
    public class ButtonTimer : NetworkBehaviour, IInteractable
    {
        [SerializeField] private IActivable[] _activablesList;
        [SerializeField, Min(0f)] private float _timerDuration;
        [Networked(OnChanged = nameof(OnInteractedChanged), OnChangedTargets = OnChangedTargets.InputAuthority)] private NetworkBool _hasBeenPressed { get; set; }
        private float _currentTime;

        public void Interact()
        {
            if (!_hasBeenPressed)
            {
                for (int i = 0; i < _activablesList.Length; i++)
                {
                    _activablesList[i].Activate();
                }
                _hasBeenPressed = true;
                StartCoroutine(Timer());
            }
        }

        private IEnumerator Timer()
        {
            while (_currentTime < _timerDuration)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime >= _timerDuration)
                {
                    _hasBeenPressed = false;
                }
                yield return null;
            }
        }

        private static void OnInteractedChanged(Changed<ButtonTimer> changed)
        {
            changed.Behaviour.UpdateFeedback(changed.Behaviour._hasBeenPressed);
        }
        /// <summary>
        /// This method will play any feedbacks that needs to hapen when this object changes ex: particles, materias, sounds etc
        /// </summary>
        /// <param name="isActive"></param>
        private void UpdateFeedback(bool isActive)
        {
            //TODO SEE WHAT WILL CHANGE IN VISUAL
            transform.localScale = isActive ? Vector3.one : Vector3.one / 2;
        }
    }
}