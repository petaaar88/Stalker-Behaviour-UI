using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PanelSettings
{
    public CanvasGroup canvasGroup;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
}

public class GenericPanelSwitcher : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField]
    private PanelSettings[] panelSettings;

    [Header("Default Settings")]
    [SerializeField]
    private float defaultFadeInDuration = 0.5f;

    [SerializeField]
    private float defaultFadeOutDuration = 0.5f;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onPanelSwitchStart;

    [SerializeField]
    private UnityEvent onPanelSwitchComplete;

    private int currentPanelIndex = 0;

    private void Start()
    {
        for (int i = 0; i < panelSettings.Length; i++)
        {
            if (i == 0)
            {
                panelSettings[i].canvasGroup.alpha = 1;
                panelSettings[i].canvasGroup.gameObject.SetActive(true);
            }
            else
            {
                panelSettings[i].canvasGroup.alpha = 0;
                panelSettings[i].canvasGroup.gameObject.SetActive(false);
            }
        }
    }

    public void SwitchToPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panelSettings.Length || panelIndex == currentPanelIndex)
            return;

        onPanelSwitchStart?.Invoke();

        PanelSettings currentPanelSettings = panelSettings[currentPanelIndex];
        PanelSettings targetPanelSettings = panelSettings[panelIndex];

        CanvasGroup currentPanel = currentPanelSettings.canvasGroup;
        CanvasGroup targetPanel = targetPanelSettings.canvasGroup;

        float fadeOutDuration = currentPanelSettings.fadeOutDuration;
        float fadeInDuration = targetPanelSettings.fadeInDuration;

        currentPanel.DOFade(0, fadeOutDuration).OnComplete(() =>
        {
            currentPanel.gameObject.SetActive(false);
            targetPanel.gameObject.SetActive(true);
            targetPanel.alpha = 0;
            targetPanel.DOFade(1, fadeInDuration).OnComplete(() =>
            {
                currentPanelIndex = panelIndex;
                onPanelSwitchComplete?.Invoke();
            });
        });
    }

    public void SwitchToNextPanel()
    {
        int nextIndex = (currentPanelIndex + 1) % panelSettings.Length;
        SwitchToPanel(nextIndex);
    }

    public void SwitchToPreviousPanel()
    {
        int prevIndex = currentPanelIndex - 1;
        if (prevIndex < 0)
            prevIndex = panelSettings.Length - 1;
        SwitchToPanel(prevIndex);
    }

    public void SwitchPanels(int fromIndex, int toIndex)
    {
        if (fromIndex != currentPanelIndex)
            return;

        SwitchToPanel(toIndex);
    }

    public int GetCurrentPanelIndex()
    {
        return currentPanelIndex;
    }

    public CanvasGroup GetCurrentPanel()
    {
        return panelSettings[currentPanelIndex].canvasGroup;
    }

    public void SetPanelFadeInDuration(int panelIndex, float duration)
    {
        if (panelIndex >= 0 && panelIndex < panelSettings.Length)
            panelSettings[panelIndex].fadeInDuration = duration;
    }

    public void SetPanelFadeOutDuration(int panelIndex, float duration)
    {
        if (panelIndex >= 0 && panelIndex < panelSettings.Length)
            panelSettings[panelIndex].fadeOutDuration = duration;
    }

    public void SetAllPanelsFadeInDuration(float duration)
    {
        for (int i = 0; i < panelSettings.Length; i++)
            panelSettings[i].fadeInDuration = duration;
    }

    public void SetAllPanelsFadeOutDuration(float duration)
    {
        for (int i = 0; i < panelSettings.Length; i++)
            panelSettings[i].fadeOutDuration = duration;
    }

    public void AddPanel(CanvasGroup panel, float fadeIn = -1, float fadeOut = -1)
    {
        PanelSettings newSettings = new PanelSettings();
        newSettings.canvasGroup = panel;
        newSettings.fadeInDuration = fadeIn < 0 ? defaultFadeInDuration : fadeIn;
        newSettings.fadeOutDuration = fadeOut < 0 ? defaultFadeOutDuration : fadeOut;

        System.Array.Resize(ref panelSettings, panelSettings.Length + 1);
        panelSettings[panelSettings.Length - 1] = newSettings;

        panel.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    public int GetPanelCount()
    {
        return panelSettings.Length;
    }

    public PanelSettings GetPanelSettings(int index)
    {
        if (index >= 0 && index < panelSettings.Length)
            return panelSettings[index];
        return null;
    }
}