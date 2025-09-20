using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro; 

public class TextFadeAndSceneChange : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI uiText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private string nextSceneName = "NextScene";

    private void Start()
    {
        if (uiText == null)
        {
            Debug.LogError("Nije povezan UI Text!");
            return;
        }

        PlayTextAnimation();
    }

    private void PlayTextAnimation()
    {
        Color c = uiText.color;
        c.a = 0f;
        uiText.color = c;

        Sequence seq = DOTween.Sequence();

        seq.Append(uiText.DOFade(1f, fadeInDuration))       
           .AppendInterval(displayDuration)                
           .Append(uiText.DOFade(0f, fadeOutDuration))     
           .OnComplete(() =>
           {
               SceneManager.LoadScene(nextSceneName);
           });
    }
}
