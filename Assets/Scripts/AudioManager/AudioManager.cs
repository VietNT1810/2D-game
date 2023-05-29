using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------ Audio Source ------")]
    //[SerializeField] private Sound[] musicSounds, sfxSounds;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("------ Audio Clip ------")]
    public AudioClip background;
    public AudioClip jump;
    public AudioClip fruit;
    public AudioClip death;
    public AudioClip finish;

    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
