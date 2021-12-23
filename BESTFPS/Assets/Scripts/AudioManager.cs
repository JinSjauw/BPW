using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    private static GameObject oneShotObject;
    private static AudioSource oneShotAudioSource;
    private static AudioSource continousAudioSource;


    public static void PlaySound(AudioClip audioClip) 
    {
        if (audioClip == null) return;

        if (oneShotObject == null) 
        {
            oneShotObject = new GameObject("One Shot Sound");
            oneShotAudioSource = oneShotObject.AddComponent<AudioSource>();
        }
        oneShotAudioSource.PlayOneShot(audioClip);
    }
    public static void PlaySoundOnce(AudioClip audioClip, Vector3 position) 
    {
        if (audioClip == null) return;

        if (oneShotObject == null) 
        {
            oneShotObject = new GameObject("One Shot Sound");
            oneShotAudioSource = oneShotObject.AddComponent<AudioSource>();
        }
        oneShotObject.transform.position = position;
        oneShotAudioSource.Stop();
        oneShotAudioSource.PlayOneShot(audioClip);
       
    }

    public static void PlaySound(AudioClip audioClip, Vector3 position)
    {
        if (audioClip == null) return;
        GameObject soundObject = new GameObject("Sound");
        soundObject.transform.position = position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();

        Object.Destroy(soundObject, audioSource.clip.length);
    }
}
