using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Size : NetworkBehaviour {

    [Header("Parameters")]

    [SerializeField] private SizeType _sizeType;
    public enum SizeType {
        S,
        M,
        B
    }
    [SerializeField] private bool _triPhase;
    [SerializeField] private float _triPhaseTransitionDuration;
    private bool _isTransitioning = false;

    [Space(16)]

    [Tooltip("[3] Small, Medium, Big")]
    [SerializeField] private float[] _sizeScales = new float[3];
    private Vector3[] _sizeScalesVector = new Vector3[3];

    [Header("Cache")]

    private float _progressC;
    private Vector3 _sizeInitialC;
    private Vector3 _sizeFinalC;

    // Access
    public SizeType Type { get { return _sizeType; } }
    public bool TriPhase { get { return _triPhase; } }
    
    public override void Spawned() {
        for(int i = 0; i < _sizeScales.Length; i++) _sizeScalesVector[i] = Vector3.one * _sizeScales[i];

        transform.localScale = _sizeScalesVector[(int)_sizeType];
    }

    public void ChangeSize(bool isGrowing) {
        StartCoroutine(ChangeSizeRoutine(isGrowing));
    }

    private IEnumerator ChangeSizeRoutine(bool isGrowing) {
        _isTransitioning = true;

        if (isGrowing) {
            if (_sizeType != SizeType.B) _sizeType++;
        }
        else {
            if (_sizeType != SizeType.S) _sizeType--;
        }

        _progressC = 0;
        _sizeInitialC = transform.localScale;
        _sizeFinalC = _sizeScalesVector[(int)_sizeType];

        while (_progressC < 1) {
            _progressC += Time.deltaTime / _triPhaseTransitionDuration;
            transform.localScale = Vector3.Lerp(_sizeInitialC, _sizeFinalC, _progressC);

            yield return null;
        }
        transform.localScale = _sizeFinalC;

        _isTransitioning = false;
    }
    
}
