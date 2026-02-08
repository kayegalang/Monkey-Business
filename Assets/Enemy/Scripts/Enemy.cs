using System.Collections;
using Enemy.Test;
using UnityEngine;

namespace Enemy.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Configuration")]
        public EnemyData enemyData;
        
        [Header("Currency")]
        [SerializeField] private BananaWallet wallet;
        
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        
        [Header("Health Bar")]
        [SerializeField] private GameObject healthBarPrefab; 
        private EnemyHealthBar healthBar;
        
        // States
        private enum EnemyState { Moving, Fighting }
        private EnemyState currentState = EnemyState.Moving;
        
        // Combat
        private float currentHealth;
        private float nextAttackTime;
        private Transform player;
        private Coroutine flashCoroutine;
        private Color originalColor;
        
        [SerializeField] private LayerMask playerLayer;
        
        void Start()
        {
            InitializeEnemy();
        }
        
        void Update()
        {
            switch (currentState)
            {
                case EnemyState.Moving:
                    MoveRight();
                    CheckForPlayer();
                    break;
                    
                case EnemyState.Fighting:
                    FightPlayer();
                    break;
            }
        }
        
        void InitializeEnemy()
        {
            currentHealth = enemyData.health;
            
            if (spriteRenderer != null && enemyData.enemySprite != null)
            {
                spriteRenderer.sprite = enemyData.enemySprite;
                GenerateCollider();
            }
            
            if (animator != null && enemyData.animatorController != null)
            {
                animator.runtimeAnimatorController = enemyData.animatorController;
            }
            
            nextAttackTime = Time.time;
            
            SpawnHealthBar();
            
            originalColor = spriteRenderer.color;
        }
        
        void SpawnHealthBar()
        {
            if (healthBarPrefab != null)
            {
                GameObject healthBarObj = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
                healthBar = healthBarObj.GetComponent<EnemyHealthBar>();
                
                if (healthBar != null)
                {
                    healthBar.SetTarget(transform);
                    healthBar.UpdateHealth(currentHealth, enemyData.health);
                }
            }
        }
        
        void GenerateCollider()
        {
            gameObject.AddComponent<PolygonCollider2D>();
        }
        
        void MoveRight()
        {
            transform.Translate(Vector2.right * enemyData.speed * Time.deltaTime);

            StartWalkingAnimation();
        }

        private void StartWalkingAnimation()
        {
            if (animator != null)
            {
                animator.SetBool("IsWalking", true);
            }
        }

        void CheckForPlayer()
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, 
                Vector2.right, 
                enemyData.range,
                playerLayer
            );
            
            if (hit.collider != null)
            {
                player = hit.collider.transform;
                EnterCombat();
            }
        }
        
        void EnterCombat()
        {
            currentState = EnemyState.Fighting;
            
            StopWalkingAnimation();
        }

        private void StopWalkingAnimation()
        {
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
        }

        void FightPlayer()
        {
            if (player == null || Vector2.Distance(transform.position, player.position) > enemyData.range)
            {
                player = null;
                currentState = EnemyState.Moving;
                return;
            }
            
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + enemyData.cooldown;
            }
        }
        
        void AttackPlayer()
        {
            if (player == null) return;
            
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyData.damage);
            }
            
            TriggerAttackAnimation();
        }

        private void TriggerAttackAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;

            FlashRed();
            
            UpdateHealthBar();
            
            TriggerHurtAnimation();
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void FlashRed()
        {
            if (spriteRenderer == null) return;

            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }

            flashCoroutine = StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            spriteRenderer.color = Color.red;
            
            yield return new WaitForSeconds(0.2f);
            
            spriteRenderer.color = originalColor;
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.UpdateHealth(currentHealth, enemyData.health);
            }
        }

        private void TriggerHurtAnimation()
        {
            
        }

        void Die()
        {
            if (healthBar != null)
            {
                Destroy(healthBar.gameObject);
            }
            
            spriteRenderer.color = originalColor;
            
            StopCoroutine(flashCoroutine);
            
            TriggerDeathAnimation();

            DropBananas();
            
            Destroy(gameObject, 1.07f);
        }

        private void TriggerDeathAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
        }

        private void DropBananas()
        {
            if (wallet != null)
            {
                wallet.Add(enemyData.currencyAmount);
                Debug.Log("Dropped " + enemyData.currencyAmount + " bananas");
            }
            else
            {
                Debug.LogWarning("No BananaWallet on " + gameObject.name);
            }
        }
    }
}