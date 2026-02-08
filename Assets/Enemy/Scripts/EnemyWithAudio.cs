using Audio.Scripts;
using Enemy.Test;
using UnityEngine;

namespace Enemy.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Configuration")]
        public EnemyData enemyData;
        
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
        
        [SerializeField] private LayerMask playerLayer;
        
        // Audio
        [SerializeField] private AudioSource walkSoundSource;
        [SerializeField] private AudioClipData hitSound;
        [SerializeField] private AudioClipData deathSound;
        
        void Start()
        {
            InitializeEnemy();
            PlayWalkingSound();
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

        private void PlayWalkingSound()
        {
            walkSoundSource.Play();
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
            
            StopWalkingSound();
        }

        private void StopWalkingSound()
        {
            walkSoundSource.Stop();
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
                PlayWalkingSound();
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

            PlayAttackSound();
        }

        private void PlayAttackSound()
        {
            AudioManager.Instance.PlaySound(hitSound);        
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
            
            UpdateHealthBar();
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.UpdateHealth(currentHealth, enemyData.health);
            }
        }

        void Die()
        {
            if (healthBar != null)
            {
                Destroy(healthBar.gameObject);
            }
            
            TriggerDeathAnimation();

            PlayDeathSound();

            DropBananas();
            
            Destroy(gameObject, 0.5f);
        }

        private void PlayDeathSound()
        {
            AudioManager.Instance.PlaySound(deathSound);
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
            Debug.Log("Dropped " + enemyData.currencyAmount + " bananas");
        }
    }
}