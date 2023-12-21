using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource BGM;
    [SerializeField] AudioSource SFX;
    [SerializeField] AudioSettings[] sounds;
    [SerializeField] int test;

    public enum Sounds
    {
        Walking,
        OptionPick,
        OptionHover,
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

    }

    [ContextMenu("test sfx")]
    public void Test()
    {
        PlaySound((Sounds)test);
    }

    private void Start()
    {
        BGM.Play();
    }

    public void ToggleSound(bool toggle)
    {
        int volume = toggle ? 0 : -80;
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void PlaySound(Sounds soundName)
    {
        AudioSettings AS = sounds[(int)soundName];
        SFX.clip = AS.clip;
        SFX.volume = AS.volume;
        SFX.pitch = AS.pitch;
        SFX.Play();
    }

    public void PlayTransitionSound()
    {
        StartCoroutine(nameof(TransitionSound));
    }

    IEnumerator TransitionSound()
    {
        AudioSettings AS = sounds[(int)Sounds.Walking];
        SFX.clip = AS.clip;
        SFX.volume = AS.volume;
        SFX.pitch = AS.pitch;
        SFX.time = Random.Range(0, 9);
        DOTween.To(() => SFX.volume, x => SFX.volume = x, 0.2f, 3);
        SFX.Play();
        yield return new WaitForSeconds(3);
        SFX.Stop();
    }
}

[System.Serializable]
public class AudioSettings
{
    public AudioClip clip;
    public float volume;
    public float pitch;
}