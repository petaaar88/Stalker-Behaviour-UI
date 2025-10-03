using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Brightness : MonoBehaviour
{

    [SerializeField]
    private Slider brightnessSlider;

    public PostProcessVolume postProcessVolume;

    public PostProcessLayer layer;

    private AutoExposure autoExposure;

    void Start()
    {
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out autoExposure);

            if (autoExposure != null)
            {
                autoExposure.keyValue.overrideState = true;

                brightnessSlider.value = autoExposure.keyValue.value;
            }
        }
    }

    void Update()
    {
        // Nema promena
    }

    public void AdjuctstBrightness(float value)
    {
        if (autoExposure != null)
        {

            if (value != 0)
                autoExposure.keyValue.value = value;
            else
                autoExposure.keyValue.value = 0.5f;
        }
    }
}