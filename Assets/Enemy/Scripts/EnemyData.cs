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
        public float health = 3f;
        public float damage = 0.5f;
        public float cooldown = 3f;
        public float speed = 1f;
        public float range = 1f;
        public int currencyAmount = 1;

        [Header("Identity")]
        public string enemyId = "enemy";
        public string displayName = "Enemy";
        
        [Header("Health Bar")]
        [Tooltip("Where to position the health bar relative to enemy center")]
        public Vector3 healthBarOffset = new Vector3(0, 1f, 0);
    }
}