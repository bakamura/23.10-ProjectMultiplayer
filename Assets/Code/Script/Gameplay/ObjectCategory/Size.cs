using Fusion;
using UnityEngine;
using System.Collections;

namespace ProjectMultiplayer.ObjectCategory.Size
{
    public class Size : NetworkBehaviour
    {

        [Header("Parameters")]

        [SerializeField] private SizeType _sizeType;
        public enum SizeType
        {
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

        //[Header("Cache")]

        //private float _progressC;
        //private Vector3 _sizeInitialC;
        //private Vector3 _sizeFinalC;
        //[Networked] private TickTimer _sizeChangeTimer { get; set; }


#if UNITY_EDITOR
        [Header("Debug")]

        [SerializeField] private bool _debugLogs;
#endif

        // Access
        public SizeType Type { get { return _sizeType; } }
        public bool TriPhase { get { return _triPhase; } }

        public override void Spawned()
        {
            for (int i = 0; i < _sizeScales.Length; i++) _sizeScalesVector[i] = Vector3.one * _sizeScales[i];

            transform.localScale = _sizeScalesVector[(int)_sizeType];
#if UNITY_EDITOR
            if (_debugLogs) Debug.Log($"{gameObject.name}'s Size initialized with {_sizeScales.Length} sizes and started with size {_sizeType}");
#endif
        }

        //        public override void FixedUpdateNetwork() {
        //            if (_sizeChangeTimer.IsRunning) {
        //#if UNITY_EDITOR
        //                if(_debugLogs) Debug.Log($"{gameObject.name} currently changing size to {_sizeType}");
        //#endif
        //                _progressC += Time.fixedDeltaTime / _triPhaseTransitionDuration;
        //                transform.localScale = Vector3.Lerp(_sizeInitialC, _sizeFinalC, _progressC);

        //                if (_sizeChangeTimer.RemainingTicks(Runner) <= 1) {
        //                    _sizeChangeTimer = TickTimer.None;
        //                    transform.localScale = _sizeFinalC;
        //                    _isTransitioning = false;
        //                }
        //            }
        //        }

        private IEnumerator ChangingSize(Vector3 initialSize, Vector3 finalSize)
        {
            WaitForFixedUpdate delay = new WaitForFixedUpdate();
            float progress = 0;
            while (progress < 1)
            {
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} currently changing size to {_sizeType}");
#endif
                progress += Time.fixedDeltaTime / _triPhaseTransitionDuration;
                transform.localScale = Vector3.Lerp(initialSize, finalSize, progress);
                yield return delay;
            }
            _isTransitioning = false;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void Rpc_UpdateSize(Vector3 initialSize, Vector3 finalSize)
        {
            StartCoroutine(ChangingSize(initialSize, finalSize));
        }

        public void ChangeSize(bool isGrowing)
        {
            if (!_isTransitioning)
            {
                _isTransitioning = true;

                if (isGrowing)
                {
                    if (_sizeType != SizeType.B) _sizeType++;
                }
                else
                {
                    if (_sizeType != SizeType.S) _sizeType--;
                }
#if UNITY_EDITOR
                if (_debugLogs) Debug.Log($"{gameObject.name} trying to change size to {_sizeType}");
#endif

                //_progressC = 0;
                //_sizeInitialC = transform.localScale;
                //_sizeFinalC = _sizeScalesVector[(int)_sizeType];
                //StartCoroutine(ChangingSize(transform.localScale, _sizeScalesVector[(int)_sizeType]));
                //_sizeChangeTimer = TickTimer.CreateFromSeconds(Runner, _triPhaseTransitionDuration);
                Rpc_UpdateSize(transform.localScale, _sizeScalesVector[(int)_sizeType]);
            }
        }

    }
}
