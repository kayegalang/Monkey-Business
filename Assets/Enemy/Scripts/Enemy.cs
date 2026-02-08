using System.Collections;
using Audio.Scripts;
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
        [SerializeField] private GameObject bananaPrefab; 

        [Header("References")] 
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        [Header("Health Bar")] 
        [SerializeField] private GameObject healthBarPrefab;

        [SerializeField] private Vector3 healthBarOffset;
        private EnemyHealthBar healthBar;

        // States
        private enum EnemyState
        {
            Moving,
            Fighting
        }

        private EnemyState currentState = EnemyState.Moving;

        // Combat
        private float currentHealth;
        private float nextAttackTime;
        private Transform player;
        private Coroutine flashCoroutine;
        private Color originalColor;

        [SerializeField] private LayerMask playerLayer;

        // Audio
        [SerializeField] private AudioClipData hitSound;
        [SerializeField] private AudioClipData deathSound;
        [SerializeField] private AudioSource walkingAudioSource;
        private AudioSource currentAttackSource;

        void Start()
        {
            InitializeEnemy();
        }

        void Update()
        {
            switch (currentState)
            {
                case EnemyState.Moving:
                    PlayWalkingSound();
                    MoveRight();
                    CheckForPlayer();
                    break;

                case EnemyState.Fighting:
                    StopWalkingSound();
                    FightPlayer();
                    break;
            }
        }

        private void PlayWalkingSound()
        {
            if (walkingAudioSource != null && !walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Play();
            }
        }

        private void StopWalkingSound()
        {
            if (walkingAudioSource != null && walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Stop();
            }
        }

        void InitializeEnemy()
        {
            currentHealth = enemyData.health;
            
            healthBarOffset = enemyData.healthBarOffset;

            if (spriteRenderer != null && enemyData.enemySprite != null)
            {
                spriteRenderer.sprite = enemyData.enemySprite;
            }

            if (animator != null && enemyData.animatorController != null)
            {
                animator.runtimeAnimatorController = enemyData.animatorController;
            }
            
            GenerateCollider();

            nextAttackTime = Time.time;
            SpawnHealthBar();
            originalColor = spriteRenderer.color;
        }

        void SpawnHealthBar()
        {
            if (healthBarPrefab != null)
            {
                GameObject healthBarsParent = GameObject.Find("HealthBars");

                GameObject barGO = Instantiate(
                    healthBarPrefab,
                    healthBarsParent.transform
                );

                healthBar = barGO.GetComponent<EnemyHealthBar>();
                healthBar.SetTarget(transform, healthBarOffset);
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
            PlayAttackSound();
        }

        private void PlayAttackSound()
        {
            if (hitSound != null)
            {
                // Store the AudioSource reference!
                currentAttackSource = AudioManager.Instance.PlaySound(hitSound);
            }
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
            // Animation trigger if needed
        }

        void Die()
        {
            // STOP ALL ENEMY SOUNDS IMMEDIATELY!
            StopWalkingSound();
            
            // Stop attack sound using stored reference
            if (currentAttackSource != null)
            {
                currentAttackSource.Stop();
            }
            
            if (healthBar != null)
            {
                Destroy(healthBar.gameObject);
            }
            
            spriteRenderer.color = originalColor;
            
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            
            TriggerDeathAnimation();
            
            // Small delay before death sound for clarity
            StartCoroutine(PlayDeathSoundDelayed());
            
            DropBananas();
            
            Destroy(gameObject, 1.07f);
        }

        private IEnumerator PlayDeathSoundDelayed()
        {
            // Wait just a tiny bit to ensure attack sound stops
            yield return new WaitForSeconds(0.05f);
            
            if (deathSound != null)
            {
                AudioManager.Instance.PlaySound(deathSound);
            }
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
                if (bananaPrefab != null)
                    Instantiate(bananaPrefab, transform.position, Quaternion.identity);
                Debug.Log("Dropped " + enemyData.currencyAmount + " bananas");
            }
            else
            {
                Debug.LogWarning("No BananaWallet on " + gameObject.name);
            }
        }
    }
}