using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum EAudioMixerType
{
    Master,
    BGM,
    SFX
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixer audioMixer;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private float[] audioVolumes = new float[3];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        for (int i = 0; i < audioClips.Length; i++)
        {
            if (scene.name == audioClips[i].name)
            {
                audioSource.clip = audioClips[i];
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    public void OnEndBGM()
    {
        audioSource.clip = audioClips[audioClips.Length - 1];
        audioSource.loop = true;
        audioSource.volume = 0.1f;
        audioSource.Play();
    }
}
