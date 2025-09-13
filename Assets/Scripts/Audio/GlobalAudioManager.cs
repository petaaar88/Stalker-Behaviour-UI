using UnityEngine;
using System.Collections.Generic;

public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance;

    [Header("Master Volume Controls")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    [Range(0f, 1f)]
    public float vfxVolume = 1f;

    // Lista svih ObjectAudioManager-a koji se registruju
    private List<ObjectAudioManager> registeredAudioManagers = new List<ObjectAudioManager>();

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterAudioManager(ObjectAudioManager audioManager)
    {
        if (!registeredAudioManagers.Contains(audioManager))
        {
            registeredAudioManagers.Add(audioManager);
        }
    }

    public void UnregisterAudioManager(ObjectAudioManager audioManager)
    {
        registeredAudioManagers.Remove(audioManager);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllAudioManagers();
    }

    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        UpdateAllAudioManagers();
    }

    public void SetVFXVolume(float volume)
    {
        vfxVolume = Mathf.Clamp01(volume);
        UpdateAllAudioManagers();
    }

    public float GetVolumeForType(SoundType soundType)
    {
        float typeVolume = (soundType == SoundType.Sound) ? soundVolume : vfxVolume;
        return masterVolume * typeVolume;
    }

    void UpdateAllAudioManagers()
    {
        // Ukloni null reference-e (objekti koji su uni�teni)
        registeredAudioManagers.RemoveAll(item => item == null);

        // A�uriraj sve registrovane AudioManager-e
        foreach (ObjectAudioManager audioManager in registeredAudioManagers)
        {
            if (audioManager != null)
            {
                audioManager.UpdateVolumes();
            }
        }
    }

    // Update za real-time promene u Inspector-u
    void Update()
    {
        UpdateAllAudioManagers();
    }
}