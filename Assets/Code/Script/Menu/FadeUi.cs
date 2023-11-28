using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fusion;
using Fusion.Sockets;
using ProjectMultiplayer.Connection;

namespace ProjectMultiplayer.UI
{
    public class FadeUI : NetworkBehaviour
    {
        [SerializeField, Min(0f)] private float _defaultFadeDuration = 1f;
        [SerializeField] private bool _startWithFadeOut = true;

        private Image _fadeImage;
        private const float _fadeUpdateFrequency = .02f;
        private Coroutine _fadeCoroutine;
        private WaitForSeconds _delay;
        public Action OnFadeEnd;
        private bool _initialFadOutDone;

        public enum FadeTypes
        {
            FADEIN,
            FADEOUT
        }

        private void Awake()
        {
            _fadeImage = GetComponent<Image>();
            _delay = new WaitForSeconds(_fadeUpdateFrequency);
        }

        public override void FixedUpdateNetwork()
        {
            if (_startWithFadeOut && !_initialFadOutDone)
            {
                _initialFadOutDone = true;
                Rpc_ChangeFade(FadeTypes.FADEOUT);
            }
        }

        private void UpdateFade(FadeTypes fadeType, /*Action OnFadeEnd = null,*/ float fadeDuration = 0)
        {
            if (_fadeCoroutine == null)
            {
                _fadeImage.raycastTarget = true;
                _fadeCoroutine = StartCoroutine(FadeUICoroutine(fadeType, /*OnFadeEnd,*/ fadeDuration));
            }
        }

        IEnumerator FadeUICoroutine(FadeTypes fadeType, /*Action OnFadeEnd = null,*/ float fadeDuration = 0)
        {
            float delta = 0;
            float durationFactor = fadeDuration > 0 ? fadeDuration : _defaultFadeDuration;
            Color currentColor = fadeType == FadeTypes.FADEIN ? Color.clear : Color.black;
            while (delta < 1)
            {
                _fadeImage.color = Color.Lerp(currentColor, fadeType == FadeTypes.FADEIN ? Color.black : Color.clear, delta);
                delta += _fadeUpdateFrequency / durationFactor;
                yield return _delay;
            }
            _fadeImage.raycastTarget = fadeType == FadeTypes.FADEIN;
            _fadeCoroutine = null;
            OnFadeEnd?.Invoke();
            //OnFadeEnd?.Invoke();
        }

        private void RemoveInitialFadeOut()
        {
            NetworkManagerReference.Instance.OnFixedNetworkUpdate -= RemoveInitialFadeOut;
            Rpc_ChangeFade(FadeTypes.FADEOUT);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_ChangeFade(FadeTypes fadeType, /*Action OnFadeEnd = null,*/ float fadeDuration = 0)
        {
            UpdateFade(fadeType, fadeDuration);
        }
    }
}
