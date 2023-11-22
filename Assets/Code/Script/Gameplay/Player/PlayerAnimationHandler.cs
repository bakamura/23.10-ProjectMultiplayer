using ProjectMultiplayer.Player;
using System.Collections;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour {

    [System.Serializable]
    struct FaceAnimation {
        public string animationName;
        public Sprite[] faceSprite;
        public float[] faceTime;
        public bool loop;
    }

    [Header("Faces")]

    [SerializeField] private FaceAnimation[] _faceAnimations;
    private int _faceAnimationCurrent = 0;

    [Header("Cache")]

    protected Animator _animator;
    protected Player _player;
    protected SpriteRenderer _faceSpriteRenderer;

    protected void Awake() {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
        _faceSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Start() {
        StartCoroutine(FaceAnimationRoutine(_faceAnimations[_faceAnimationCurrent]));
    }

    protected void Update() {
        _animator.SetBool("isMoving", Mathf.Abs(_player.NRigidbody.Rigidbody.velocity.x) + Mathf.Abs(_player.NRigidbody.Rigidbody.velocity.z) > 0.1f);
        _animator.SetBool("onAir ", Mathf.Abs(_player.NRigidbody.Rigidbody.velocity.y) > 0.1f);

        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != _faceAnimations[0].animationName) {
            for (int i = 0; i < _faceAnimations.Length; i++) {
                if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == _faceAnimations[i].animationName) {
                    _faceAnimationCurrent = i;
                    StopAllCoroutines();
                    StartCoroutine(FaceAnimationRoutine(_faceAnimations[_faceAnimationCurrent]));
                }
            }
        }
    }

    public void SetTrigger(string trigger) {
        _animator.SetTrigger(trigger);
    }

    private IEnumerator FaceAnimationRoutine(FaceAnimation animation) {
        _faceSpriteRenderer.sprite = animation.faceSprite[0];
        if (animation.faceSprite.Length > 1) {
            float faceTimer = 0;
            int faceCurrent = 1;
            while (true) {
                faceTimer += Time.deltaTime;
                if(faceTimer > animation.faceTime[faceCurrent]) {
                    faceCurrent++;
                    if (faceCurrent == animation.faceSprite.Length) break;
                    _faceSpriteRenderer.sprite = animation.faceSprite[faceCurrent];
                }

                yield return null;
            }
        }

        if(animation.loop) StartCoroutine(FaceAnimationRoutine(_faceAnimations[_faceAnimationCurrent]));
    }
}

