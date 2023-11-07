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
        private bool _hasBeenPressed;
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
            while(_currentTime < _timerDuration)
            {
                _currentTime += Time.deltaTime;
                if(_currentTime >= _timerDuration)
                {
                    _hasBeenPressed = false;
                }
                yield return null;
            }
        }

    }
}