using UnityEngine;
using System.Collections;
using Audio.Scripts;

/// <summary>
/// Player Monkey - DPS is now purely calculated (Damage × APS)
/// DPS upgrade button just increases attack speed!
/// </summary>
public class PlayerMonkey : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private BananaWallet wallet;
    
    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    [Header("Health Bar")]
    [SerializeField] private Transform healthBarFill;
    
    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource walkingAudioSource;
    private AudioSource currentAttackSource;
    
    // Current stats (updated from MonkeyStatsManager)
    private float maxHealth;
    private float currentHealth;
    private float damage;           // Damage per hit
    private float attacksPerSecond; // Attacks per second
    private float moveSpeed;
    private float attackRange;
    
    // Derived stats
    private float dps;              // Calculated: damage × attacksPerSecond
    private float attackCooldown;   // Calculated: 1 / attacksPerSecond
    
    // State
    private enum State { Moving, Fighting }
    private State currentState = State.Moving;
    
    // Combat
    private Transform currentTarget;
    private float nextAttackTime;
    
    void Start()
    {
        UpdateStatsFromManager();
        currentHealth = maxHealth;
        UpdateHealthBar();
        
        if (MonkeyStatsManager.Instance != null)
        {
            MonkeyStatsManager.Instance.OnStatsChanged += UpdateStatsFromManager;
            MonkeyStatsManager.Instance.OnMonkeyEvolved += OnEvolved;
        }
    }
    
    void OnDestroy()
    {
        if (MonkeyStatsManager.Instance != null)
        {
            MonkeyStatsManager.Instance.OnStatsChanged -= UpdateStatsFromManager;
            MonkeyStatsManager.Instance.OnMonkeyEvolved -= OnEvolved;
        }
    }
    
    void UpdateStatsFromManager()
    {
        if (MonkeyStatsManager.Instance == null) return;
        
        // Get base stats
        maxHealth = MonkeyStatsManager.Instance.GetCurrentHealth();
        damage = MonkeyStatsManager.Instance.GetCurrentDamage();
        attacksPerSecond = MonkeyStatsManager.Instance.GetCurrentAttackSpeed();
        moveSpeed = MonkeyStatsManager.Instance.GetCurrentMoveSpeed();
        attackRange = MonkeyStatsManager.Instance.GetCurrentRange();
        
        // Calculate derived stats
        dps = MonkeyStatsManager.Instance.GetCurrentDPS(); // Damage × APS
        attackCooldown = attacksPerSecond > 0 ? 1f / attacksPerSecond : 1f;
        
        Debug.Log($"Player stats updated: HP:{maxHealth}, DMG:{damage}, APS:{attacksPerSecond}, DPS:{dps:F1}, Speed:{moveSpeed}, Range:{attackRange}");
        Debug.Log($"Attack cooldown: {attackCooldown:F2}s (attacking {attacksPerSecond:F2} times per second)");
    }
    
    void OnEvolved()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        Debug.Log("Player evolved! Healed to full!");
    }
    
    void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                PlayWalkingSound();
                MoveLeft();
                CheckForEnemy();
                break;
            case State.Fighting:
                StopWalkingSound();
                FightEnemy();
                break;
        }
    }
    
    void PlayWalkingSound()
    {
        if (walkingAudioSource != null && !walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Play();
        }
    }
    
    void StopWalkingSound()
    {
        if (walkingAudioSource != null && walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Stop();
        }
    }
    
    void MoveLeft()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        StartWalkingAnimation();
    }
    
    void StartWalkingAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }
    }
    
    void CheckForEnemy()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.left,
            attackRange,
            enemyLayer
        );
        
        if (hit.collider != null)
        {
            currentTarget = hit.collider.transform;
            EnterCombat();
        }
    }
    
    void EnterCombat()
    {
        currentState = State.Fighting;
        StopWalkingAnimation();
    }
    
    void StopWalkingAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }
    
    void FightEnemy()
    {
        if (currentTarget == null || Vector2.Distance(transform.position, currentTarget.position) > attackRange)
        {
            currentTarget = null;
            currentState = State.Moving;
            return;
        }
        
        if (Time.time >= nextAttackTime)
        {
            AttackEnemy();
            nextAttackTime = Time.time + attackCooldown;
        }
    }
    
    void AttackEnemy()
    {
        if (currentTarget == null) return;
        
        float totalDamage = damage;
        
        var enemy = currentTarget.GetComponent<Enemy.Scripts.Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(totalDamage);
            Debug.Log($"Player attacks for {totalDamage} damage! (Cooldown: {attackCooldown:F2}s, DPS: {dps:F1})");
        }
        
        TriggerAttackAnimation();
        PlayAttackSound();
    }
    
    void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }
    
    void PlayAttackSound()
    {
        if (attackSound != null)
        {
            currentAttackSource = AudioManager.Instance?.PlaySound(attackSound);
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        UpdateHealthBar();
        FlashRed();
        
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        if (hurtSound != null && currentHealth > 0)
        {
            AudioManager.Instance?.PlaySound(hurtSound);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void FlashRed()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashCoroutine());
        }
    }
    
    IEnumerator FlashCoroutine()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = original;
    }
    
    void UpdateHealthBar()
    {
        if (healthBarFill != null && maxHealth > 0)
        {
            float fillAmount = currentHealth / maxHealth;
            healthBarFill.localScale = new Vector3(fillAmount, 1f, 1f);
        }
    }
    
    void Die()
    {
        Debug.Log("Player monkey died!");
        
        StopWalkingSound();
        if (currentAttackSource != null)
        {
            currentAttackSource.Stop();
        }
        
        TriggerDeathAnimation();
        StartCoroutine(PlayDeathSoundDelayed());
        
        enabled = false;
        StartCoroutine(GameOver());
    }
    
    IEnumerator PlayDeathSoundDelayed()
    {
        yield return new WaitForSeconds(0.05f);
        
        if (deathSound != null)
        {
            AudioManager.Instance?.PlaySound(deathSound);
        }
    }
    
    void TriggerDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }
    
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("GAME OVER!");
        
        // GameSceneManager.Instance?.ChangeState(GameSceneManager.State.GameOver);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.left * attackRange);
    }
}