using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ProjectMultiplayer.UI
{
    public class FadeUi : MonoBehaviour
    {
        public static FadeUi Instance { get; private set; }

        [SerializeField, Min(0f)] private float _defaultFadeDuration = 1f;

        private Image _fadeImage;
        private const float _fadeUpdateFrequency = .02f;
        private Coroutine _fadeCoroutine;
        private WaitForSeconds _delay;

        public enum FadeTypes
        {
            FADEIN,
            FADEOUT
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);
            _fadeImage = GetComponent<Image>();
            _delay = new WaitForSeconds(_fadeUpdateFrequency);
        }

        public void UpdateFade(FadeTypes fadeType, Action OnFadeEnd = null, float fadeDuration = 0)
        {
            if (_fadeCoroutine == null)
            {
                _fadeImage.raycastTarget = true;
                _fadeCoroutine = StartCoroutine(FadeUI(fadeType, OnFadeEnd, fadeDuration));
            }
        }

        IEnumerator FadeUI(FadeTypes fadeType, Action OnFadeEnd = null, float fadeDuration = 0)
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
        }
    }
}
