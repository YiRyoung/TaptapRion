using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum SoundType
{
    BGM,
    SFX
}

public class SoundSystem : MonoBehaviour
{
    public SoundType soundType;
    public AudioMixer audioMixer;
    public Slider audioSlider;
    string TypeName;

    void Type()
    {
        switch(soundType)
        {
            case SoundType.BGM:
                TypeName = "BGM";
                break;
            case SoundType.SFX:
                TypeName = "SFX";
                break;
        }
    }

    public void AudioControl()
    {
        Type();
        float sound = audioSlider.value;

        if(sound == -40f) { audioMixer.SetFloat(TypeName, -80); }
        else { audioMixer.SetFloat(TypeName, sound); }
    }
}
