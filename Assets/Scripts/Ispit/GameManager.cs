using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Health playerHealth;
   private ThrowingObject throwingObject;

    [SerializeField]
    private bool hasKey = false;

    [SerializeField]
    private string youDiedSceneName = "YouDied";

    [SerializeField]
    private Text healthText;
    [SerializeField]
    private Text bottles;
    [SerializeField]
    private Text objective;

    void Awake()
    {
        Cursor.visible = false;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<Health>();
        }

        throwingObject = playerHealth.GetComponentInChildren<ThrowingObject>();
        objective.text = "Objective: Find the key to unlock the door.";
    }

    void Update()
    {
        if(playerHealth != null)
            healthText.text = "Health: " + playerHealth.GetHealth().ToString();

        bottles.text = "Bottles: " + (throwingObject != null ? (throwingObject.HasInfiniteProjectiles() ? "∞" : throwingObject.GetNumberOfProjectiles().ToString()) : "0");

        if (playerHealth != null && playerHealth.IsDead)
            LoadYouDiedScene();
    }

    public void SetKeyCollected(bool collected)
    {
        hasKey = collected;
        objective.text = "Objective: Find Door.";

        Debug.Log($"Key collected: {hasKey}");
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public bool OpenDoor()
    {
        if (hasKey)
        {
            Debug.Log("Door opened successfully!");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("WinScene");
            return true;
        }
        else
        {
            Debug.Log("Door is locked. You need a key!");
            return false;
        }
    }

    private void LoadYouDiedScene()
    {
        Debug.Log("Player died. Loading YouDied scene...");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(youDiedSceneName);
    }

    public Health GetPlayerHealth()
    {
        return playerHealth;
    }

    public void SetPlayerHealth(Health health)
    {
        playerHealth = health;
    }
}