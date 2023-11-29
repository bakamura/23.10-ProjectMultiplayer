using Fusion;
using ProjectMultiplayer.Player;
using System.Collections;
using UnityEngine;

public class PlayerAnimationHandler : NetworkBehaviour {

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

    protected NetworkMecanimAnimator _animator;
    protected Player _player;
    protected SpriteRenderer _faceSpriteRenderer;

    public override void Spawned() {
        _animator = GetComponent<NetworkMecanimAnimator>();
        _player = transform.parent.GetComponent<Player>();
        _faceSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(FaceAnimationRoutine(_faceAnimations[_faceAnimationCurrent]));
    }

    public override void FixedUpdateNetwork() {
        _animator
            .Animator
            .SetBool("isMoving", Mathf.Abs(
                _player
                .NRigidbody
                .Rigidbody.velocity.x) + Mathf.Abs(
                    _player
                    .NRigidbody
                    .Rigidbody.velocity.z) > 0.1f);
        _animator.Animator.SetBool("onAir ", Mathf.Abs(_player.NRigidbody.Rigidbody.velocity.y) > 0.1f);

        if (_animator.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != _faceAnimations[0].animationName) {
            for (int i = 0; i < _faceAnimations.Length; i++) {
                if (_animator.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == _faceAnimations[i].animationName) {
                    _faceAnimationCurrent = i;
                    StopAllCoroutines();
                    StartCoroutine(FaceAnimationRoutine(_faceAnimations[_faceAnimationCurrent]));
                }
            }
        }
    }

    public void SetTrigger(string trigger) {
        if(trigger.Length > 0) _animator.Animator.SetTrigger(trigger);
    }

    public void SetBool(string boolName, bool isTrue) {
        _animator.Animator.SetBool(boolName, isTrue);
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

