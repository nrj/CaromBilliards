using UnityEngine;
using System;
using System.Collections.Generic;
using strange.extensions.context.api;

public class AudioService
{
    [Inject(ContextKeys.CONTEXT_VIEW)]
    public GameObject contextView { get; set; }

    public AudioSource audioSource;
    public AudioLibrary audioLibrary;

    [PostConstruct]
    public void PostConstruct()
    {
        foreach (var go in contextView.scene.GetRootGameObjects())
        {
            // Get a reference to the AudioLibrary and AudioSource components from the the root scene
            if (go.name == "AudioLibrary")
            {
                audioSource = go.GetComponent<AudioSource>();
                audioLibrary = go.GetComponent<AudioLibrary>();
                Debug.Log("AudioLibrary - initialized");
                break;
            }
        }
    }

    public void PlaySound(SoundName sound, float volumeScale)
    {
        var audioItem = audioLibrary.GetItem(sound);
        if (audioItem != null)
        {
            audioSource.PlayOneShot(audioItem.clip, volumeScale);
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }
}