using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform fillRect; 
    [SerializeField] private Image fillImage; 
    // [SerializeField] private TextMeshProUGUI healthText; 
    
    [Header("Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f; 
    
    [Header("Animation")]
    [SerializeField] private bool animateChange = true;
    [SerializeField] private float animationSpeed = 5f;
    
    private float targetScale = 1f;
    private float currentScale = 1f;
    
    void Update()
    {
        if (animateChange && fillRect != null)
        {
            currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * animationSpeed);
            
            Vector3 scale = fillRect.localScale;
            scale.x = currentScale;
            fillRect.localScale = scale;
        }
    }
    
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (fillRect == null) return;
        
        targetScale = Mathf.Clamp01(currentHealth / maxHealth);
        
        if (!animateChange)
        {
            currentScale = targetScale;
            Vector3 scale = fillRect.localScale;
            scale.x = currentScale;
            fillRect.localScale = scale;
        }
        
        if (fillImage != null)
        {
            if (targetScale <= lowHealthThreshold)
            {
                fillImage.color = lowHealthColor;
            }
            else
            {
                fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, 
                    (targetScale - lowHealthThreshold) / (1f - lowHealthThreshold));
            }
        }
        
        // if (healthText != null)
        // {
        //     healthText.text = $"HP: {Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
        // }
    }
    
    public void SetHealthImmediate(float currentHealth, float maxHealth)
    {
        targetScale = Mathf.Clamp01(currentHealth / maxHealth);
        currentScale = targetScale;
        
        if (fillRect != null)
        {
            Vector3 scale = fillRect.localScale;
            scale.x = currentScale;
            fillRect.localScale = scale;
        }
        
        UpdateHealth(currentHealth, maxHealth);
    }
}