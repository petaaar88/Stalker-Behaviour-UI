using System.Collections;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField]
    private GameObject canvas;

    private bool isPlayerNearby = false;
    private ObjectAudioManager audioManager;
    private Renderer[] renderers;
    private Animator playerAnimator;


    void Start()
    {
        audioManager = GetComponent<ObjectAudioManager>();
        renderers = GetComponentsInChildren<Renderer>();
        playerAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();

    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            canvas.SetActive(false);
            CollectKey();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            isPlayerNearby = true;
            Debug.Log("Press " + interactionKey + " to pick up the key.");
            canvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            canvas.SetActive(false);
            isPlayerNearby = false;
        }
    }

    private void CollectKey()
    {
        Debug.Log("Key collected!");
        playerAnimator.SetTrigger("IsPickup");

        if (GameManager.Instance != null)
            GameManager.Instance.SetKeyCollected(true);
        else
            Debug.LogError("GameManager instance not found!");

        foreach (var rend in renderers)
            rend.enabled = false;

        audioManager.PlaySound("Pickup");

        StartCoroutine(DestroyAfterSound("Pickup"));
    }

    private IEnumerator DestroyAfterSound(string soundName)
    {
        while (audioManager.IsSoundPlaying(soundName))
            yield return null;

        Destroy(gameObject);
    }
}
