using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    private bool isPaused;
    [SerializeField]
    private GameObject pauseCanvas;

    [SerializeField]
    private GameObject hudCanvas;

    [SerializeField]
    private GameObject mainGroup;

    private AudioPauseManager audioPauseManager;

    // Start is called before the first frame update
    void Start()
    {
        audioPauseManager = FindObjectOfType<AudioPauseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                isPaused = false;
                Time.timeScale = 1;
                pauseCanvas.SetActive(false);
                hudCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                audioPauseManager.ResumeAll();
            }
            else
            {
                audioPauseManager.PauseAllExcept();
                isPaused = true;
                Time.timeScale = 0;
                pauseCanvas.SetActive(true);
                hudCanvas.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioPauseManager.ResumeAll();
    }
}
