using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Brightness : MonoBehaviour
{

    [SerializeField]
    private Slider brightnessSlider;

    public PostProcessProfile brightness;
    public PostProcessLayer layer;

    private AutoExposure autoExposure;
    // Start is called before the first frame update
    void Start()
    {
        brightness.TryGetSettings(out autoExposure);
        brightnessSlider.value = autoExposure.keyValue.value;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AdjuctstBrightness(float value)
    {
        if (value != 0)
            autoExposure.keyValue.value = value;
        else
            autoExposure.keyValue.value = 0.5f;
    }
}