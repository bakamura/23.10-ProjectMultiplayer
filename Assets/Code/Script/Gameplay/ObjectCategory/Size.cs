using Fusion;
using UnityEngine;

namespace ProjectMultiplayer.ObjectCategory.Size {
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
        [Networked] private TickTimer _sizeChangeTimer { get; set; }

        // Access
        public SizeType Type { get { return _sizeType; } }
        public bool TriPhase { get { return _triPhase; } }

        public override void Spawned() {
            for (int i = 0; i < _sizeScales.Length; i++) _sizeScalesVector[i] = Vector3.one * _sizeScales[i];

            transform.localScale = _sizeScalesVector[(int)_sizeType];
        }

        public override void FixedUpdateNetwork() {
            if (_sizeChangeTimer.IsRunning) {
                _progressC += Time.fixedDeltaTime / _triPhaseTransitionDuration;
                transform.localScale = Vector3.Lerp(_sizeInitialC, _sizeFinalC, _progressC);

                if (_sizeChangeTimer.RemainingTicks(Runner) <= 1) {
                    _sizeChangeTimer = TickTimer.None;
                    transform.localScale = _sizeFinalC;
                    _isTransitioning = false;
                }
            }
        }

        public void ChangeSize(bool isGrowing) {
            if (!_isTransitioning) {
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
                _sizeChangeTimer = TickTimer.CreateFromSeconds(Runner, _triPhaseTransitionDuration);
            }
        }

    }
}
