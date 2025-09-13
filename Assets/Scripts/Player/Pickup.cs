using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private ThrowingObject throwingObject;
    private ObjectAudioManager audioManager;
    private bool isInRange = false;
    private Renderer[] childRenderers;
    private Animator playerAnimator;

    void Start()
    {
        audioManager = GetComponent<ObjectAudioManager>();
        throwingObject = FindObjectOfType<ThrowingObject>();
        childRenderers = GetComponentsInChildren<Renderer>(); 

        playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (throwingObject.AddProjectile())
            {
                playerAnimator.SetTrigger("IsPickup");
                foreach (var rend in childRenderers)
                    rend.enabled = false;

                audioManager.PlaySound("Pickup");

                StartCoroutine(DestroyAfterSound("Pickup"));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isInRange = false;
    }

    private IEnumerator DestroyAfterSound(string soundName)
    {
        while (audioManager.IsSoundPlaying(soundName))
            yield return null;

        Destroy(gameObject);
    }
}
