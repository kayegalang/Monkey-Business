using UnityEngine;

namespace Enemy.Scripts
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Monkey Business/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Visual")]
        public Sprite enemySprite;
        public RuntimeAnimatorController animatorController;
    
        [Header("Stats")]
        public float moveSpeed = 2f;
        public float maxHealth = 100f;
        public float attack = 10f;
        public float defense = 10f;
    
        [Header("Rewards")]
        public int bananaDropAmount = 5;
    
        [Header("Combat")]
        public float range = 1f; 
        public float cooldown = 1f;
        public float dps = 1f;
        public float aps = 1f;
    }
}