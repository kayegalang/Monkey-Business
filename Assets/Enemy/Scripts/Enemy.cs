using UnityEngine;

namespace Enemy.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Configuration")]
        public EnemyData enemyData;
        
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        
        private float currentHealth;
        private float lastAttackTime;
        
        private readonly Vector2 moveDirection = Vector2.right;
        
        void Start()
        {
            InitializeEnemy();
        }
        
        void Update()
        {
            MoveEnemy();
        }
        
        void InitializeEnemy()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            //animator = GetComponent<Animator>();
            
            currentHealth = enemyData.maxHealth;
            
            if (spriteRenderer != null && enemyData.enemySprite != null)
            {
                spriteRenderer.sprite = enemyData.enemySprite;
            }
            
            if (animator != null && enemyData.animatorController != null)
            {
                animator.runtimeAnimatorController = enemyData.animatorController;
            }
        }
        
        void MoveEnemy()
        {
            transform.Translate(moveDirection * enemyData.moveSpeed * Time.deltaTime);
        }
        
        void AttackPlayer()
        {
            // TODO
        }
        
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        void Die()
        {
            Destroy(gameObject);
        }
    }
}