using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider masterVolumeSlider;
    public Slider soundVolumeSlider;
    public Slider vfxVolumeSlider;

    [Header("Optional: Volume Display")]
    public TMPro.TextMeshProUGUI masterVolumeText;
    public TMPro.TextMeshProUGUI soundVolumeText;
    public TMPro.TextMeshProUGUI vfxVolumeText;

    void Start()
    {
        if (GlobalAudioManager.Instance == null)
        {
            Debug.LogError("GlobalAudioManager not found! Please create a GameObject with GlobalAudioManager script.");
            return;
        }

        // Postaviti početne vrednosti slider-a
        InitializeSliders();

        // Dodeli event listener-e za slider-e
        SetupSliderListeners();

        // Početno ažuriranje teksta
        UpdateVolumeTexts();
    }

    void InitializeSliders()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = GlobalAudioManager.Instance.masterVolume;

        if (soundVolumeSlider != null)
            soundVolumeSlider.value = GlobalAudioManager.Instance.soundVolume;

        if (vfxVolumeSlider != null)
            vfxVolumeSlider.value = GlobalAudioManager.Instance.vfxVolume;
    }

    void SetupSliderListeners()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (soundVolumeSlider != null)
        {
            soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
        }

        if (vfxVolumeSlider != null)
        {
            vfxVolumeSlider.onValueChanged.AddListener(OnVFXVolumeChanged);
        }
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (GlobalAudioManager.Instance != null)
        {
            GlobalAudioManager.Instance.SetMasterVolume(value);
            UpdateVolumeTexts();
        }
    }

    public void OnSoundVolumeChanged(float value)
    {
        if (GlobalAudioManager.Instance != null)
        {
            GlobalAudioManager.Instance.SetSoundVolume(value);
            UpdateVolumeTexts();
        }
    }

    public void OnVFXVolumeChanged(float value)
    {
        if (GlobalAudioManager.Instance != null)
        {
            GlobalAudioManager.Instance.SetVFXVolume(value);
            UpdateVolumeTexts();
        }
    }

    void UpdateVolumeTexts()
    {
        if (GlobalAudioManager.Instance != null)
        {
            if (masterVolumeText != null)
                masterVolumeText.text = $"Master: {(GlobalAudioManager.Instance.masterVolume * 100):F0}%";

            if (soundVolumeText != null)
                soundVolumeText.text = $"Sound: {(GlobalAudioManager.Instance.soundVolume * 100):F0}%";

            if (vfxVolumeText != null)
                vfxVolumeText.text = $"VFX: {(GlobalAudioManager.Instance.vfxVolume * 100):F0}%";
        }
    }

    // Dodatne korisne metode
    public void ResetAllVolumes()
    {
        OnMasterVolumeChanged(1f);
        OnSoundVolumeChanged(1f);
        OnVFXVolumeChanged(1f);

        if (masterVolumeSlider != null) masterVolumeSlider.value = 1f;
        if (soundVolumeSlider != null) soundVolumeSlider.value = 1f;
        if (vfxVolumeSlider != null) vfxVolumeSlider.value = 1f;
    }

    public void MuteAll()
    {
        OnMasterVolumeChanged(0f);
        if (masterVolumeSlider != null) masterVolumeSlider.value = 0f;
    }
}