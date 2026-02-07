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
        
        [Header("Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0);
        [SerializeField] private float heightOffset = 1f; 
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private float maxWidth = 1f; 
        
        private Transform target; 
        
        void Start()
        {
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.WorldSpace;
            }
        }
        
        void LateUpdate()
        {
            if (target != null)
            {
                Vector3 finalOffset = new Vector3(offset.x, heightOffset, offset.z);
                transform.position = target.position + finalOffset;
            }
        }
        
        public void SetTarget(Transform enemyTransform)
        {
            target = enemyTransform;
        }
        
        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (fillRect == null) return;
            
            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
            
            Vector3 scale = fillRect.localScale;
            scale.x = healthPercent * maxWidth;
            fillRect.localScale = scale;
            
            if (hideWhenFull && canvas != null)
            {
                canvas.gameObject.SetActive(healthPercent < 1f);
            }
            
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(Color.red, Color.green, healthPercent);
            }
        }
    } 
}