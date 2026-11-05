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

        [Space(10)]
        [SerializeField] private Turret _turretPref;

        [Space(10)]
        [SerializeField] private uint _relocateNumber;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public uint Durability => _durability;
        public uint ShootDamage => _shootDamage;
        public float RateOfFire => _rateOfFire;
        public uint RelocateNumber => _relocateNumber;
        public ProjectileSchema ProjectileScheme => _projectileScheme;
        public Turret TurretPrefab => _turretPref;
        #endregion
    }
}