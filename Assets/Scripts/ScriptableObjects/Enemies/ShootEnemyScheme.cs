using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewShootEnemy", menuName = "Enemies/ShootEnemy", order = 10)]
    public class ShootEnemyScheme : EnemyScheme
    {
        #region FIELDS INSPECTOR
        [SerializeField] private uint _durability;

        [Space(10)]
        [SerializeField] private uint _shootDamage;
        [SerializeField] private float _rateOfFire;
        [SerializeField] private ProjectileSchema _projectileScheme;
        #endregion

        #region PROPERTIES
        public uint Durability => _durability;
        public uint ShootDamage => _shootDamage;
        public float RateOfFire => _rateOfFire;
        public ProjectileSchema ProjectileScheme => _projectileScheme;
        #endregion
    }
}