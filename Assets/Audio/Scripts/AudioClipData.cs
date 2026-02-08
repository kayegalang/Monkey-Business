using UnityEngine;

namespace Audio.Scripts
{
    [CreateAssetMenu(fileName = "AudioClipData", menuName = "Monkey Business/Audio Clip Data")]

    public class AudioClipData : ScriptableObject
    {
        [Header("Clip Info")]
        public string clipName;
        public AudioClip audioClip;
    
        [Header("Settings")]
        [Range(0f, 1f)]
        public float volume = 1f;
    
        [Range(0.1f, 3f)]
        public float pitch = 1f;
    
        [Tooltip("Randomize pitch slightly for variation")]
        public bool randomizePitch = false;
    
        [Tooltip("How much to randomize pitch (±)")]
        [Range(0f, 0.3f)]
        public float pitchVariation = 0.1f;
    
        public bool loop = false;
    
        [Range(0f, 500f)]
        public float maxDistance = 50f;
    }
}