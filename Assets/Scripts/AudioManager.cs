using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField]
    AudioSource[] AudioSources;
    // Use this for initialization
    void Start()
    {
        AudioSources = GetComponentsInChildren<AudioSource>();
    }

    public void PlaySfx(AudioClip clip)
    {
        foreach (AudioSource source in AudioSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                break;
            }
        }
    }
}
