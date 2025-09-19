using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "MultipleStalkersScene";
    [SerializeField] private float delay = 2f;

    public void ChangeScene(string sceneName)
    {
        if (sceneName == mainSceneName)
        {
            StartCoroutine(ChangeSceneCoroutine(sceneName));
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }
}
