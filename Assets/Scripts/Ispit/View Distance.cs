using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewDistance : MonoBehaviour
{
    [SerializeField] private Slider fogSlider;
    [SerializeField] private float minFog = 0.03f;
    [SerializeField] private float maxFog = 0.2f;

    private static float savedFogDensity = -1f; 
    private static float originalFogIntensity;

    void Start()
    {
        RenderSettings.fog = true;

        if (savedFogDensity >= 0)
        {
            originalFogIntensity = savedFogDensity;
            RenderSettings.fogDensity = savedFogDensity;
        }
        else
        {
            originalFogIntensity = RenderSettings.fogDensity;
            savedFogDensity = originalFogIntensity;
        }

        if (fogSlider != null)
        {
            fogSlider.minValue = minFog;
            fogSlider.maxValue = maxFog;
            fogSlider.value = Mathf.Clamp(originalFogIntensity, minFog, maxFog);
            fogSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void OnSliderChanged(float value)
    {
        RenderSettings.fogDensity = value;
        savedFogDensity = value; 
    }

    public void ResetFog()
    {
        if (fogSlider != null)
        {
            fogSlider.value = Mathf.Clamp(originalFogIntensity, minFog, maxFog);
        }
    }
}