using UnityEngine;

namespace Test
{
    public class SimplePlayer : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 1f;
        
        [Header("Combat")]
        [SerializeField] private float attackDamage = 1f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackCooldown = 1f;
        
        [Header("Detection")]
        [SerializeField] private LayerMask enemyLayer;
        
        // State
        private enum PlayerState { Moving, Fighting }
        private PlayerState currentState = PlayerState.Moving;
        
        // Combat
        private Transform enemy; 
        private float nextAttackTime;
        
        void Start()
        {
            nextAttackTime = Time.time;
        }
        
        void Update()
        {
            switch (currentState)
            {
                case PlayerState.Moving:
                    MoveLeft();
                    CheckForEnemy();
                    break;
                    
                case PlayerState.Fighting:
                    FightEnemy();
                    break;
            }
        }
        
        void MoveLeft()
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
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
                enemy = hit.collider.transform;
                EnterCombat();
            }
        }
        
        void EnterCombat()
        {
            currentState = PlayerState.Fighting;
        }
        
        void FightEnemy()
        {
            if (enemy == null || Vector2.Distance(transform.position, enemy.position) > attackRange)
            {
                enemy = null;
                currentState = PlayerState.Moving;
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
            if (enemy == null) return;
            
            Enemy.Scripts.Enemy target = enemy.GetComponent<Enemy.Scripts.Enemy>();
            if (enemy != null)
            {
                target.TakeDamage(attackDamage);
            }
        }
    }
}