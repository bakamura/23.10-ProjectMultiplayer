using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace ProjectMultiplayer.UI
{
    public class Menu : MonoBehaviour
    {

        [SerializeField] private float _openDuration;
        [SerializeField, Tooltip("if contais something and the player tries to return on the UI, this Canvas will never close if is the only one open")] private CanvasGroup _alwaysStayOnThisUI;
        protected Coroutine _canvasTransitionCoroutine = null;
        //private WaitForSeconds _delay = new WaitForSeconds(_canvasTick);
        //private const float _canvasTick = .02f;
        protected Stack<CanvasGroup> _currentCanvasOpened = new Stack<CanvasGroup>();
        protected Action<CanvasGroup> _onTransitionEnd;

        protected virtual void Awake()
        {
            if (_alwaysStayOnThisUI) _currentCanvasOpened.Push(_alwaysStayOnThisUI);
        }

        public void OpenCanvas(CanvasGroup canvasToOpen)
        {

        }

        public void ChangeCurrentCanvas(CanvasGroup canvasCurrentNew)
        {
            //prevents the player to activate transition when he is already in the only UI available
            if ((_currentCanvasOpened.Count == 1 && _alwaysStayOnThisUI && canvasCurrentNew == _currentCanvasOpened.First()) || _canvasTransitionCoroutine != null) return;
            _canvasTransitionCoroutine = StartCoroutine(CanvasTransition(canvasCurrentNew, 1f));
        }
        /// <summary>
        /// clears all the stack of UIs and opens AlwaysStayOnThisUI canvas, if AlwaysStayOnThisUI is null needs to recive a canvas to open
        /// </summary>
        /// <param name="canvasToReturn"></param>
        protected void ReturnToDefaultUI(CanvasGroup canvasToReturn = null)
        {
            if (_alwaysStayOnThisUI)
            {
                _onTransitionEnd += HandleCanvasStackReset;
                ChangeCurrentCanvas(_alwaysStayOnThisUI);                
            }
            else
            {
                _currentCanvasOpened.Clear();
                ChangeCurrentCanvas(canvasToReturn);
            }
        }

        private void HandleCanvasStackReset(CanvasGroup currentCanvas)
        {
            _currentCanvasOpened.Clear();
            _currentCanvasOpened.Push(_alwaysStayOnThisUI);
            _onTransitionEnd -= HandleCanvasStackReset;
        }

        protected CanvasGroup GetPreviousCanvasGroup(out CanvasGroup result)
        {
            if (_currentCanvasOpened.Count == 0) return result = null;
            if (_alwaysStayOnThisUI)
            {
                result = _currentCanvasOpened.Count - 1 > 0 ? _currentCanvasOpened.ElementAt(1) : null;
            }
            else
            {
                result = _currentCanvasOpened.ElementAt(1);
            }
            return result;
        }

        private IEnumerator CanvasTransition(CanvasGroup newCanvas, float targetAlpha)
        {
            float step = Time.deltaTime / _openDuration;
            float currentStep = 0;
            float currentAlpha;

            if (_currentCanvasOpened.Count > 0)
            {
                CanvasGroup temp = _currentCanvasOpened.Peek();
                currentAlpha = temp.alpha;
                while (currentStep < 1)
                {
                    temp.alpha = Mathf.Lerp(currentAlpha, 0f, currentStep);
                    currentStep += step;
                    yield return null;
                }
                temp.alpha = 0f;
                temp.blocksRaycasts = false;
                temp.interactable = false;
            }

            if (_currentCanvasOpened.Contains(newCanvas))
            {
                _currentCanvasOpened.Pop();
            }
            else
            {
                _currentCanvasOpened.Push(newCanvas);
            }

            currentStep = 0;
            currentAlpha = newCanvas.alpha;

            while (currentStep < 1)
            {
                newCanvas.alpha = Mathf.Lerp(currentAlpha, targetAlpha, currentStep);
                currentStep += step;
                yield return null;
            }
            newCanvas.alpha = targetAlpha;
            newCanvas.blocksRaycasts = true;
            newCanvas.interactable = true;

            _onTransitionEnd?.Invoke(newCanvas);
            _canvasTransitionCoroutine = null;
        }

    }
}