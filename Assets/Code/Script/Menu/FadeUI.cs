using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fusion;

namespace ProjectMultiplayer.UI
{
    public class FadeUI : NetworkBehaviour
    {
        [SerializeField, Min(0f)] private float _defaultFadeDuration = 1f;
        [SerializeField] private bool _startWithFadeOut = true;

        private AudioSource _musicToFade;
        private Image _fadeImage;
        private const float _fadeUpdateFrequency = .02f;
        private Coroutine _fadeCoroutine;
        private WaitForSeconds _delay;
        public Action OnFadeEnd;

        public enum FadeTypes
        {
            FADEIN,
            FADEOUT
        }

        private void Awake()
        {
            _musicToFade = GetComponent<AudioSource>();
            _fadeImage = GetComponent<Image>();
            _delay = new WaitForSeconds(_fadeUpdateFrequency);
        }

        public override void Spawned()
        {
            if (_startWithFadeOut)
                UpdateFade(FadeTypes.FADEOUT);
        }
        //public override void FixedUpdateNetwork()
        //{
        //    if (_startWithFadeOut && !_initialFadOutDone)
        //    {
        //        _initialFadOutDone = true;
        //        Rpc_ChangeFade(FadeTypes.FADEOUT);
        //    }
        //}

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
            Color currentColor = _fadeImage.color;
            while (delta < 1)
            {
                delta += _fadeUpdateFrequency / durationFactor;
                _fadeImage.color = Color.Lerp(currentColor, fadeType == FadeTypes.FADEIN ? Color.black : Color.clear, delta);
                if (_musicToFade.clip) _musicToFade.volume = fadeType == FadeTypes.FADEIN ? 1f - delta : delta;                
                yield return _delay;
            }
            _fadeImage.raycastTarget = fadeType == FadeTypes.FADEIN;
            _fadeCoroutine = null;
            OnFadeEnd?.Invoke();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void Rpc_ChangeFade(FadeTypes fadeType, /*Action OnFadeEnd = null,*/ float fadeDuration = 0)
        {
            UpdateFade(fadeType, fadeDuration);
        }
    }
}