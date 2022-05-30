using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewBulletEnemy", menuName = "BulletEnemy", order = 11)]
    public class BulletEnemy : Enemy
    {
        #region FIELDS INSPECTOR
        [Space(10)]
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private Bullet _bulletPrefab;

        [Space(10)]
        [SerializeField] private float _speed;
        [SerializeField] private uint _damage;
        [SerializeField] private float _rateOfFire;

        [Space(10)]
        [SerializeField] private CarLevel _level;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public uint Damage => _damage;
        public float RateOfFire => _rateOfFire;

        public CarLevel Level => _level;
        public float BulletSpeed => _bulletSpeed;
        public Bullet BulletPrefab => _bulletPrefab;
        #endregion
    }
}
