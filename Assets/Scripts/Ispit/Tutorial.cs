using System.Collections;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private float delay = 3f;
    private PauseScript pauseScript;
    [SerializeField]
    private GameObject gameUI;

    private bool isEnabled = true;
    [SerializeField] private TextMeshProUGUI enableTutorialButtonText;

    private static Tutorial instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        enableTutorialButtonText.text = isEnabled ? "ON" : "OFF";

        if (isEnabled)
        {
            if (tutorialCanvas != null)
                StartCoroutine(EnableAfterDelay());
            pauseScript = FindObjectOfType<PauseScript>();
        }
    }

    private IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        gameUI.SetActive(false);

        if (tutorialCanvas != null)
            tutorialCanvas.SetActive(true);
        if (pauseScript != null)
            pauseScript.PauseWithoutMenu();
    }

    public void CloseTutorial()
    {
        if (tutorialCanvas != null)
            tutorialCanvas.SetActive(false);

        if (pauseScript != null)
            pauseScript.UnpauseWithoutMenu();

        gameUI.SetActive(true);
    }

    public void EnableDisableTutorial()
    {
        if (isEnabled)
        {
            isEnabled = false;
            enableTutorialButtonText.text = "OFF";
        }
        else
        {
            isEnabled = true;
            enableTutorialButtonText.text = "ON";
        }
    }
}
