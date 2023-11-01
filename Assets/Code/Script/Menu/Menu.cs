using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Menu : MonoBehaviour
{

    [SerializeField] private float _openDuration;
    [SerializeField, Tooltip("if contais something and the player tries to return on the UI, this Canvas will never close if is the only one open")] private CanvasGroup _alwaysStayOnThisUI;
    private Coroutine _canvasTransitionCoroutine = null;
    //private WaitForSeconds _delay = new WaitForSeconds(_canvasTick);
    //private const float _canvasTick = .02f;
    protected Stack<CanvasGroup> currentCanvasOpened = new Stack<CanvasGroup>();

    protected virtual void Awake()
    {
        if (_alwaysStayOnThisUI) currentCanvasOpened.Push(_alwaysStayOnThisUI);
    }

    public void OpenCanvas(CanvasGroup canvasToOpen)
    {

    }

    public void ChangeCurrentCanvas(CanvasGroup canvasCurrentNew)
    {
        //prevents the player to activate transition when he is already in the only UI available
        if ((currentCanvasOpened.Count == 1 && _alwaysStayOnThisUI && canvasCurrentNew == currentCanvasOpened.First()) || _canvasTransitionCoroutine != null) return;
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
            ChangeCurrentCanvas(_alwaysStayOnThisUI);
            currentCanvasOpened.Clear();
            currentCanvasOpened.Push(_alwaysStayOnThisUI);
        }
        else
        {
            currentCanvasOpened.Clear();
            ChangeCurrentCanvas(canvasToReturn);
        }
    }

    protected CanvasGroup GetPreviousCanvasGroup(out CanvasGroup result)
    {
        if (currentCanvasOpened.Count == 0) return result = null;
        if (_alwaysStayOnThisUI)
        {
            result = currentCanvasOpened.Count - 1 > 0 ? currentCanvasOpened.ElementAt(1) : null;
        }
        else
        {
            result = currentCanvasOpened.ElementAt(1);
        }
        return result;
    }    

    private IEnumerator CanvasTransition(CanvasGroup newCanvas, float targetAlpha)
    {
        float step = Time.deltaTime / _openDuration;
        float currentStep = 0;
        float currentAlpha;

        if (currentCanvasOpened.Count > 0)
        {
            CanvasGroup temp = currentCanvasOpened.Peek();
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

        if (currentCanvasOpened.Contains(newCanvas))
        {
            currentCanvasOpened.Pop();
        }
        else
        {
            currentCanvasOpened.Push(newCanvas);
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

        _canvasTransitionCoroutine = null;
    }

}
