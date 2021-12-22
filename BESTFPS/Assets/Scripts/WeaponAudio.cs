using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WeaponAudio : MonoBehaviour
{
    public enum Sound
    {
        Shoot,
        Reload,
        ReloadEmpty,
    }

    public WeaponSoundAudio[] soundArray;

    public AudioClip getClip(Sound sound) 
    {
        foreach(WeaponSoundAudio weaponSoundAudio in soundArray) 
        {
            if(weaponSoundAudio.sound == sound) 
            {
                return weaponSoundAudio.audioClip;
            }
        }
        Debug.LogError("Sound: " + sound + "not found");
        return null;
    }

}
[System.Serializable]
public class WeaponSoundAudio
{
    public WeaponAudio.Sound sound;
    public AudioClip audioClip;
}


