using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ReusableScripts.SO
{
    [CreateAssetMenu(fileName = "AudioDataSO", menuName = "AudioDataSO", order = 0)]
    public class AudioDataSO : ScriptableObject
    {
        [Header("Audio Clip")]
        [SerializeField] private AudioClipData[] _audioClipDataList;

        [Header("Default Setting")] [SerializeField]
        private float _defaultVolume = 1f;

        [SerializeField] private bool _defaultLoop = false;
        [SerializeField][Range(0, .5f)] private float _pitchVariation = 0f;

        public AudioClip GetClip(string clipName)
        {
            foreach (AudioClipData audioClipData in _audioClipDataList)
            {
                if (audioClipData.ClipName.Equals(clipName, StringComparison.OrdinalIgnoreCase))
                    return audioClipData.Clip;
            }

            return null;
        }

        public AudioClip GetClip(int index)
        {
            if(index >= 0 && index < _audioClipDataList.Length)
                return _audioClipDataList[index].Clip;

            return null;
        }

        public AudioClip GetRandomClip()
        {
            if (_audioClipDataList.Length == 0 || _audioClipDataList == null)
            {
                return null;
            }

            var validClips = Array.FindAll(_audioClipDataList, clip => clip != null);
            if (validClips == null || validClips.Length == 0)
                return null;
            
            return validClips[Random.Range(0, validClips.Length)].Clip;
        }
        
    }

    [Serializable]
    public class AudioClipData
    {
        public string ClipName;
        public AudioClip Clip;
    }
}