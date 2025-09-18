using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioValues : MonoBehaviour
{
    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Slider soundSlider;

    void Start()
    {
        masterSlider.value = GlobalAudioManager.Instance.masterVolume;
        sfxSlider.value = GlobalAudioManager.Instance.vfxVolume;
        soundSlider.value = GlobalAudioManager.Instance.soundVolume;
    }

    void Update()
    {
        masterSlider.onValueChanged.AddListener(delegate { GlobalAudioManager.Instance.SetMasterVolume(masterSlider.value); });
        sfxSlider.onValueChanged.AddListener(delegate { GlobalAudioManager.Instance.SetVFXVolume(sfxSlider.value); });
        soundSlider.onValueChanged.AddListener(delegate { GlobalAudioManager.Instance.SetSoundVolume(soundSlider.value); });
    }
}
