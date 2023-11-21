using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    [SerializeField, Tooltip("will only trigger the event if the object has any of the tags, if array is null will always trigger the events")] private List<string> _filterByTags = new List<string>();
    [SerializeField] private UnityEvent _onTriggerEnter;
    [SerializeField] private UnityEvent _onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (_filterByTags.Count == 0 || _filterByTags.Contains(other.tag))
            _onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_filterByTags.Count == 0 || _filterByTags.Contains(other.tag))
            _onTriggerExit?.Invoke();
    }
}
