using UnityEngine;
using UnityEngine.UI;

namespace Enemy.Scripts
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform fillRect; 
        [SerializeField] private Image fillImage; 
        [SerializeField] private Canvas canvas;
        
        [Header("Visual Settings")]
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private float maxWidth = 1f;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private Color highHealthColor = Color.green;
        
        private Transform target;
        private Vector3 offset;

        public void SetTarget(Transform targetTransform, Vector3 followOffset)
        {
            target = targetTransform;
            offset = followOffset;
        }

        void LateUpdate()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = target.position + offset;
        }
        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (fillRect == null) return;
            
            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
            
            // Scale the fill rect based on health
            Vector3 scale = fillRect.localScale;
            scale.x = healthPercent * maxWidth;
            fillRect.localScale = scale;
            
            // Hide when full if enabled
            if (hideWhenFull && canvas != null)
            {
                canvas.gameObject.SetActive(healthPercent < 1f);
            }
            
            // Color gradient from red to green
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(lowHealthColor, highHealthColor, healthPercent);
            }
        }
    } 
}