using ReusableScripts.Core;
using ReusableScripts.SO;
using UnityEngine;

namespace ReusableScripts.Manager
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Data")] [SerializeField]
        private AudioDataSO _sfxAudioData;
        [SerializeField]  private AudioDataSO _musicAudioData;
        
        [Header("Audio Source")]
        [SerializeField] private AudioSource _bgmAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;


        public void PlayBGM(string clipName)
        {
            if(_bgmAudioSource.isPlaying)
                _bgmAudioSource.Stop();
            
            var clip = _musicAudioData.GetClip(clipName);
            
            if (clip == null)
            {
                Debug.Log($"[AudioManager] Clip {clipName} doesn't exist!]");
                return;
            }
            
            _bgmAudioSource.clip = clip;
            _bgmAudioSource.Play();
        }

        public void PlayOneShot(string clipName)
        {
            var clip = _sfxAudioData.GetClip(clipName);

            if (clip == null)
            {
                Debug.Log($"[AudioManager] Clip {clipName} doesn't exist!]");
                return;
            }

            _sfxAudioSource.PlayOneShot(clip);
        }
        
    }
}