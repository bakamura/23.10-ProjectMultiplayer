using System.Collections;
using UnityEngine;

public class Size : MonoBehaviour {

    [Header("Size")]

    [SerializeField] private SizeClass _class;
    public enum SizeClass {
        S, M, B
    }
    [SerializeField] private bool _triPhase;
    [SerializeField] private float _triPhaseTransitionDuration;
    private bool _isTransitioning = false;

    [Space(16)]

    [Tooltip("[3] Small, Medium then Big")]
    [SerializeField] private SizeScales _sizeScales;

    [Header("Cache")]

    private float _progressC;
    private Vector3 _sizeInitialC;
    private Vector3 _sizeFinalC;

    // Access

    public SizeClass Class { get { return _class; } }

    private void Awake() {
        transform.localScale = _sizeScales.sizes[(int)_class];
    }

    public bool TryChangeSize(bool isGrowing) {
        if (_triPhase) {
            if (!_isTransitioning) StartCoroutine(ChangeSize(isGrowing));
            return true;
        }
        else return false;
    }

    private IEnumerator ChangeSize(bool isGrowing) {
        _isTransitioning = true;

        if (isGrowing) {
            if (_class != SizeClass.B) _class++;
        }
        else {
            if (_class != SizeClass.S) _class--;
        }

        _progressC = 0;
        _sizeInitialC = transform.localScale;
        _sizeFinalC = _sizeScales.sizes[(int)_class];

        while (_progressC < 1) {
            _progressC += Time.deltaTime / _triPhaseTransitionDuration;
            transform.localScale = Vector3.Lerp(_sizeInitialC, _sizeFinalC, _progressC);

            yield return null;
        }
        transform.localScale = _sizeFinalC;

        _isTransitioning = false;
    }

}
