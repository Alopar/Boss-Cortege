using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewShootEnemy", menuName = "Configs/Enemies/ShootEnemy", order = -95)]
    public class ShootEnemyScheme : EnemyScheme
    {
        #region FIELDS INSPECTOR
        [Space(10)]
        [SerializeField] private float _speed;

        [Space(10)]
        [SerializeField] private uint _durability;

        [Space(10)]
        [SerializeField] private uint _shootDamage;
        [SerializeField] private float _rateOfFire;
        [SerializeField] private ProjectileSchema _projectileScheme;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public uint Durability => _durability;
        public uint ShootDamage => _shootDamage;
        public float RateOfFire => _rateOfFire;
        public ProjectileSchema ProjectileScheme => _projectileScheme;
        #endregion

        #region METHODS PUBLIC
        public override EnemyController Factory()
        {
            var enemy = Instantiate(Prefab) as ShootEnemyController;
            enemy.Config = this;

            return enemy;
        }
        #endregion
    }
}