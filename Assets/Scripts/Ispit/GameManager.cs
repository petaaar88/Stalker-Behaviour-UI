using TMPro;
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

    [Header("Health")]
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private TextMeshProUGUI bottles;
    [SerializeField]
    private TextMeshProUGUI objective;

    [SerializeField]
    private GameObject weaponIcon;
    [SerializeField]
    private GameObject throwableIcon;

    private SwitchingWeapons switchingWeapons;

    void Awake()
    {
        Cursor.visible = false;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        switchingWeapons = FindObjectOfType<SwitchingWeapons>();
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
        objective.text = "Find the key to unlock the door";
    }

    void Update()
    {
        if(playerHealth != null)
        {
            healthText.text = playerHealth.GetHealth().ToString();
            healthBarFill.fillAmount = (float)playerHealth.GetHealth() / playerHealth.GetMaxHealth();

            if(playerHealth.GetHealth() <= 20)
                healthBarFill.color = Color.red;

        }

        if (switchingWeapons.weapon.activeSelf)
        {
            weaponIcon.SetActive(true);
            throwableIcon.SetActive(false);
        }
        else
        {
            weaponIcon.SetActive(false);
            throwableIcon.SetActive(true);
        }

            bottles.text =  (throwingObject != null ? (throwingObject.HasInfiniteProjectiles() ? "∞" : throwingObject.GetNumberOfProjectiles().ToString()) : "0");

      
    }

    public void SetKeyCollected(bool collected)
    {
        hasKey = collected;
        objective.text = "Find Door";

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