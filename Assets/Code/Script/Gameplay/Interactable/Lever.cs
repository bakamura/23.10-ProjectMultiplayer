using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMultiplayer.ObjectCategory;

namespace ProjectMultiplayer.ObjectCategory
{
    public class Lever : NetworkBehaviour, IInteractable
    {
        [SerializeField] private IActivable[] _activablesList;
        private const float _internalCooldown = .02f;
        private float _lastTimeUsed;
        public void Interact()
        {
            if(_lastTimeUsed >= _internalCooldown)
            {
                for (int i = 0; i < _activablesList.Length; i++)
                {
                    _activablesList[i].Activate();
                }
                _lastTimeUsed = 0;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (_lastTimeUsed < _internalCooldown) _lastTimeUsed += Runner.DeltaTime;
        }
    }
}