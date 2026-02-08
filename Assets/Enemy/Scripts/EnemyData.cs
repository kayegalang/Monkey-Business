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
        public float aps = 0.5f;
        public float dps = 0.25f;
        public float speed = 1f;
        public float range = 1f;
        public float cooldown = 3f;
        public int currencyAmount = 1;
    }
}