using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SoundFXController : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField]
    private List<SoundEffect> soundEffects = new List<SoundEffect>();

    private void PlaySoundInternal(string soundName, float volume = 1f)
    {
        SoundEffect soundEffect = soundEffects.Find(s => s.name == soundName);

        if (soundEffect != null)
        {
            GameObject audioObject = new GameObject($"Sound_{soundName}");
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();

            audioSource.clip = soundEffect.clip;
            audioSource.volume = Mathf.Clamp01(volume);
            audioSource.Play();

            Destroy(audioObject, soundEffect.clip.length);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
        }
    }

    [Button]
    public void Play_Lack_Activate(float volume = 1f) => PlaySoundInternal("Lack_Activate", volume);
    
    [Button]
    public void Play_Lack_RevealGreen(float volume = 1f) => PlaySoundInternal("Lack_Reveal_Green", volume);

    [Button]
    public void Play_Lack_RevealRed(float volume = 1f) => PlaySoundInternal("Lack_Reveal_Red", volume);

    [Button]
    public void Play_Lack_RevealBlue(float volume = 1f) => PlaySoundInternal("Lack_Reveal_Blue", volume);

    [Button]
    public void Play_Lack_DissolveGreen(float volume = 1f) => PlaySoundInternal("Lack_Dissolve_Green", volume);

    [Button]
    public void Play_Lack_DissolveRed(float volume = 1f) => PlaySoundInternal("Lack_Dissolve_Red", volume);

    [Button]
    public void Play_Lack_DissolveBlue(float volume = 1f) => PlaySoundInternal("Lack_Dissolve_Blue", volume);
    
}