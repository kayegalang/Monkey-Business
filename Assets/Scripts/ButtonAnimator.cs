using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Animation")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.9f;
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    
    [Header("Color Animation")]
    [SerializeField] private bool enableColorChange = true;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.7f, 1f);
    [SerializeField] private float colorDuration = 0.15f;
    
    [Header("Effects")]
    [SerializeField] private bool enablePunchOnClick = true;
    [SerializeField] private float punchStrength = 0.2f;
    [SerializeField] private bool enableRotationOnHover = false;
    [SerializeField] private float rotationAmount = 5f;
    [SerializeField] private bool enableBounceLoop = false;
    [SerializeField] private float bounceDelay = 3f;
    
    [Header("Shadow/Outline Effect")]
    [SerializeField] private bool enableShadowPulse = false;
    [SerializeField] private Shadow shadowComponent;
    
    private Vector3 originalScale;
    private Image buttonImage;
    private Color originalColor;
    private TextMeshProUGUI buttonText;
    private Sequence bounceSequence;
    private bool isHovering;

    private void Start()
    {
        originalScale = transform.localScale;
        
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }
        
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (enableBounceLoop)
        {
            StartBounceLoop();
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
        if (buttonImage != null) buttonImage.DOKill();
        if (buttonText != null) buttonText.DOKill();
        bounceSequence?.Kill();
    }

    private void StartBounceLoop()
    {
        bounceSequence?.Kill();
        
        bounceSequence = DOTween.Sequence();
        bounceSequence.AppendInterval(bounceDelay);
        bounceSequence.Append(transform.DOScale(originalScale * 1.05f, 0.3f).SetEase(Ease.InOutSine));
        bounceSequence.Append(transform.DOScale(originalScale, 0.3f).SetEase(Ease.InOutSine));
        bounceSequence.SetLoops(-1);
        bounceSequence.SetUpdate(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        // Stop bounce loop when hovering
        if (enableBounceLoop)
        {
            bounceSequence?.Pause();
        }
        
        transform.DOKill();
        
        // Scale animation with overshoot
        transform.DOScale(originalScale * hoverScale, scaleDuration)
            .SetEase(scaleEase)
            .SetUpdate(true);
        
        // Color change
        if (buttonImage != null && enableColorChange)
        {
            buttonImage.DOKill();
            buttonImage.DOColor(hoverColor, colorDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
        
        // Text pop animation
        if (buttonText != null)
        {
            buttonText.DOKill();
            buttonText.transform.DOScale(1.05f, scaleDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
        
        // Rotation
        if (enableRotationOnHover)
        {
            transform.DORotate(new Vector3(0, 0, rotationAmount), scaleDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
        
        // Shadow pulse
        if (enableShadowPulse && shadowComponent != null)
        {
            DOTween.To(() => shadowComponent.effectDistance, 
                       x => shadowComponent.effectDistance = x, 
                       new Vector2(4, -4), 
                       0.3f)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        
        // Resume bounce loop
        if (enableBounceLoop)
        {
            bounceSequence?.Play();
        }
        
        transform.DOKill();
        
        transform.DOScale(originalScale, scaleDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
        
        if (buttonImage != null && enableColorChange)
        {
            buttonImage.DOKill();
            buttonImage.DOColor(originalColor, colorDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
        
        if (buttonText != null)
        {
            buttonText.DOKill();
            buttonText.transform.DOScale(1f, scaleDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
        
        if (enableRotationOnHover)
        {
            transform.DORotate(Vector3.zero, scaleDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
        
        if (enableShadowPulse && shadowComponent != null)
        {
            DOTween.To(() => shadowComponent.effectDistance, 
                       x => shadowComponent.effectDistance = x, 
                       new Vector2(2, -2), 
                       0.3f)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        
        transform.DOScale(originalScale * clickScale, scaleDuration * 0.5f)
            .SetEase(Ease.InQuad)
            .SetUpdate(true);
            
        // Quick color flash
        if (buttonImage != null && enableColorChange)
        {
            buttonImage.DOKill();
            buttonImage.DOColor(Color.white, 0.05f)
                .SetUpdate(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        
        if (enablePunchOnClick)
        {
            transform.localScale = originalScale * (isHovering ? hoverScale : 1f);
            transform.DOPunchScale(Vector3.one * punchStrength, scaleDuration, 5, 0.5f)
                .SetUpdate(true);
                
            // Shake effect
            transform.DOShakeRotation(0.3f, strength: 10f, vibrato: 10)
                .SetUpdate(true);
        }
        else
        {
            float targetScale = isHovering ? hoverScale : 1f;
            transform.DOScale(originalScale * targetScale, scaleDuration)
                .SetEase(scaleEase)
                .SetUpdate(true);
        }
        
        // Color back
        if (buttonImage != null && enableColorChange)
        {
            buttonImage.DOColor(isHovering ? hoverColor : originalColor, colorDuration)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true);
        }
    }
    
    // Call this for special effects on important buttons
    public void PlayAttentionAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale * 1.2f, 0.2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale * 0.95f, 0.15f).SetEase(Ease.InQuad));
        seq.Append(transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack));
        seq.SetUpdate(true);
    }
}