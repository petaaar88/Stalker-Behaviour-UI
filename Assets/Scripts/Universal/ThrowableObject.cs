using System.Collections;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    private ObjectAudioManager audioManager;
    private bool sfxPlayed = false;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        audioManager = GetComponent<ObjectAudioManager>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public bool isCollided = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!sfxPlayed)
        {
            if (meshRenderer != null)
                meshRenderer.enabled = false;

            audioManager.PlaySound("Breaking");
            sfxPlayed = true;

            NoiceListener.Instance.RegisterLoudNoice(transform.position);

            isCollided = true;

            StartCoroutine(DestroyAfterSound("Breaking"));
        }
    }

    private IEnumerator DestroyAfterSound(string soundName)
    {
        while (audioManager.IsSoundPlaying(soundName))
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
