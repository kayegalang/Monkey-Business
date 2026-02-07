using UnityEngine;

namespace Enemy.Test
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 10f;
        private float currentHealth;
    
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerHealthBarUI healthBarUI; 
    
        void Start()
        {
            currentHealth = maxHealth;
            
            if (healthBarUI != null)
            {
                healthBarUI.SetHealthImmediate(currentHealth, maxHealth);
            }
            else
            {
                Debug.LogWarning("Health Bar UI not found!");
            }
        }
    
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            if (healthBarUI != null)
            {
                healthBarUI.UpdateHealth(currentHealth, maxHealth);
            }
            
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    
        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            
            if (healthBarUI != null)
            {
                healthBarUI.UpdateHealth(currentHealth, maxHealth);
            }
        }
    
        void Die()
        {
        
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
        
            gameObject.SetActive(false);
        }
    
        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public float GetHealthPercent() => currentHealth / maxHealth;
    }
}