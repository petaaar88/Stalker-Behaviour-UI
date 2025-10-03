using cakeslice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class XRayVision : MonoBehaviour
{
    [Header("Outline")]
    private bool isDisabled = false;

    [Header("Zoom")]
    [SerializeField] private float zoomInDistance = 2f;
    [SerializeField] private float zoomSpeed = 8f;
    private float originalDistance;
    private bool isZoomingIn = false;

    [SerializeField] private vThirdPersonCamera thiredPersonCamera;

    [Header("Post Processing")]
    [SerializeField] private PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;
    private Bloom bloom;
    private Grain grain; // novi efekat

    private float originalGamma;
    [SerializeField] private float zoomGamma = 0.8f;

    private float originalBloom;
    [SerializeField] private float zoomBloom = 5f;

    private float originalGrain;
    [SerializeField] private float zoomGrain = 0.5f; 

    [Header("Outline Settings")]
    [SerializeField] private float outlineRadius = 10f;
    private Outline[] outlines;

    private ObjectAudioManager audioManager;
    private GlobalAudioManager globalAudioManager;
    private float originalMasterVolume;

    private void Start()
    {
        outlines = FindObjectsByType<Outline>(FindObjectsSortMode.None);

        if (thiredPersonCamera != null)
            originalDistance = thiredPersonCamera.defaultDistance;

        if (postProcessVolume != null)
        {
            if (postProcessVolume.profile.TryGetSettings(out colorGrading))
                originalGamma = colorGrading.gamma.value.w;

            if (postProcessVolume.profile.TryGetSettings(out bloom))
                originalBloom = bloom.intensity.value;

            if (postProcessVolume.profile.TryGetSettings(out grain))
                originalGrain = grain.intensity.value;
        }

        audioManager = GetComponent<ObjectAudioManager>();

        globalAudioManager = FindObjectOfType<GlobalAudioManager>();
        originalMasterVolume = globalAudioManager.masterVolume;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isDisabled = false;
            isZoomingIn = true;
            audioManager.PlaySound("Swoosh");
            globalAudioManager.masterVolume = 0.2f;
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            isDisabled = true;
            isZoomingIn = false;
            audioManager.PlaySound("Swoosh");
            globalAudioManager.masterVolume = originalMasterVolume;
        }

        foreach (var item in outlines)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (distance <= outlineRadius)
                item.eraseRenderer = isDisabled == false ? false : true;
            else
                item.eraseRenderer = true;
        }

        if (thiredPersonCamera != null)
        {
            float target = isZoomingIn ? zoomInDistance : originalDistance;
            thiredPersonCamera.defaultDistance = Mathf.MoveTowards(thiredPersonCamera.defaultDistance, target, zoomSpeed * Time.deltaTime);
        }

        if (colorGrading != null)
        {
            float targetGamma = isZoomingIn ? zoomGamma : originalGamma;
            Vector4 gamma = colorGrading.gamma.value;
            gamma.w = Mathf.MoveTowards(gamma.w, targetGamma, Time.deltaTime * 1f);
            colorGrading.gamma.value = gamma;
        }

        if (bloom != null)
        {
            float targetBloom = isZoomingIn ? zoomBloom : originalBloom;
            bloom.intensity.value = Mathf.MoveTowards(bloom.intensity.value, targetBloom, Time.deltaTime * 2f);
        }

        if (grain != null)
        {
            float targetGrain = isZoomingIn ? zoomGrain : originalGrain;
            grain.intensity.value = Mathf.MoveTowards(grain.intensity.value, targetGrain, Time.deltaTime * 2f);
        }
    }
    private IEnumerator ApplyXRayVolumeAfterSwoosh()
    {
        yield return new WaitForSeconds(1.0f);
        globalAudioManager.masterVolume = 0.2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, outlineRadius);
    }
}
