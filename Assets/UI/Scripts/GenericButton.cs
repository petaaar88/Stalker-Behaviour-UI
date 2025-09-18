using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GenericButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Visual Settings")]
    [SerializeField]
    private TextMeshProUGUI tmp;

    [SerializeField]
    private Color hoverColor = new Color(197f / 255f, 197f / 255f, 197f / 255f);

    [SerializeField]
    private Color normalColor = new Color(103f / 255f, 103f / 255f, 103f / 255f);

    [Header("Animation Settings")]
    [SerializeField]
    private float animationDuration = 0.2f;

    [SerializeField]
    private Ease animationEase = Ease.OutQuad;

    [SerializeField]
    private bool animateScale = true;

    [SerializeField]
    private Vector3 hoverScale = new Vector3(1.0f, 1.0f, 1f);

    [SerializeField]
    private Vector3 normalScale = Vector3.one;

    [Header("Button Actions")]
    [SerializeField]
    private UnityEvent onButtonClick;

    [SerializeField]
    private UnityEvent onButtonHover;

    [SerializeField]
    private UnityEvent onButtonExit;

    private Tween colorTween;
    private Tween scaleTween;

    private ObjectAudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponent<ObjectAudioManager>();

        if (tmp != null)
            tmp.color = normalColor;
        transform.localScale = normalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Mouse entered {gameObject.name}");

        audioManager?.PlaySound("Hover");

        if (tmp != null)
        {
            colorTween?.Kill();
            colorTween = tmp.DOColor(hoverColor, animationDuration)
                .SetEase(animationEase)
                .SetUpdate(true); // Dodato - radi i tokom pauze
        }

        if (animateScale)
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(hoverScale, animationDuration)
                .SetEase(animationEase)
                .SetUpdate(true); // Dodato - radi i tokom pauze
        }

        onButtonHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmp != null)
        {
            colorTween?.Kill();
            colorTween = tmp.DOColor(normalColor, animationDuration)
                .SetEase(animationEase)
                .SetUpdate(true); // Dodato - radi i tokom pauze
        }

        if (animateScale)
        {
            scaleTween?.Kill();
            scaleTween = transform.DOScale(normalScale, animationDuration)
                .SetEase(animationEase)
                .SetUpdate(true); // Dodato - radi i tokom pauze
        }

        onButtonExit?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Button {gameObject.name} clicked!");
        audioManager?.PlaySound("Click");
        onButtonClick?.Invoke();

        if (tmp != null)
        {
            colorTween?.Kill();
            colorTween = tmp.DOColor(normalColor, animationDuration)
                .SetEase(animationEase)
                .SetUpdate(true); // Dodato - radi i tokom pauze
        }
    }

    public void SetHoverColor(Color color)
    {
        hoverColor = color;
    }

    public void SetNormalColor(Color color)
    {
        normalColor = color;
        if (tmp != null)
            tmp.color = normalColor;
    }

    public void SetAnimationDuration(float duration)
    {
        animationDuration = duration;
    }

    public void SetAnimationEase(Ease ease)
    {
        animationEase = ease;
    }

    public void SetHoverScale(Vector3 scale)
    {
        hoverScale = scale;
    }

    public void EnableScaleAnimation(bool enable)
    {
        animateScale = enable;
    }

    public void AddClickAction(UnityAction action)
    {
        onButtonClick.AddListener(action);
    }

    public void RemoveClickAction(UnityAction action)
    {
        onButtonClick.RemoveListener(action);
    }

    private void OnDestroy()
    {
        colorTween?.Kill();
        scaleTween?.Kill();
    }
}