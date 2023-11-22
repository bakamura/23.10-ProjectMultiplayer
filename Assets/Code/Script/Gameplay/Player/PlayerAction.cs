using UnityEngine;

namespace ProjectMultiplayer.Player.Actions {
    public abstract class PlayerAction : MonoBehaviour {

        protected Player _player;        
        [SerializeField] private AudioSource _audioSource;

        private void Start() {
            _player = GetComponent<Player>();
        }

        public abstract void DoAction(Ray cameraRay);

        public abstract void StopAction();

        protected void PlayAudio(AudioClip clip, bool overrideCurrentAudio = true)
        {            
            if (clip)
            {
                if (overrideCurrentAudio)
                {
                    _audioSource.clip = clip;
                    _audioSource.Play();
                }
                else
                {
                    if (!_audioSource.isPlaying)
                    {
                        _audioSource.clip = clip;
                        _audioSource.Play();
                    }
                }
            }
        }

    }
}
