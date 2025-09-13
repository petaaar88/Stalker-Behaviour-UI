using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private string playerTag = "Player";
    [SerializeField]
    private bool isPlayerNearby = false;
    [SerializeField]
    private KeyCode interactionKey = KeyCode.E;

    private bool isDoorOpen = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactionKey) && !isDoorOpen)
            TryOpenDoor();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            isPlayerNearby = true;
            Debug.Log("Player is near the door. Press " + interactionKey + " to interact.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
            isPlayerNearby = false;
    }

    private void TryOpenDoor()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found!");
            return;
        }

        if (GameManager.Instance.OpenDoor())
            OpenDoor();
        else
            Debug.Log("You need a key to open this door!");
    }

    private void OpenDoor()
    {
        isDoorOpen = true;
        Debug.Log("Door opened!");
    }
}