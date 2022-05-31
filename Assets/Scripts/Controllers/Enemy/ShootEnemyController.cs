using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public class ShootEnemyController : EnemyController
    {
        #region FIELDS PRIVATE
        private ShootEnemyScheme _config;

        private float _currentSpeed;
        private float _currentRateOfFire;

        private uint _shootMaxDamage;
        private uint _currentShootDamage;

        private bool _isFire = false;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Only the initial installation is available
        /// </summary>
        public ShootEnemyScheme Config
        {
            get { return _config; }
            set
            {
                if (_config == null)
                {
                    _config = value;

                    _currentSpeed = _config.Speed;
                    _currentRateOfFire = _config.RateOfFire;
                    
                    _shootMaxDamage = _config.ShootDamage;
                    _currentShootDamage = _shootMaxDamage;
                }
            }
        }
        #endregion

        #region UNITY CALLBACKS
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _currentSpeed * Time.deltaTime);

            if (transform.position == _currentPoint.transform.position)
            {
                Fire();
            }
        }
        #endregion

        #region METHODS PRIVATE
        private void Fire()
        {
            if (_isFire) return;

            _isFire = true;
            StartCoroutine(SpawnBullet(_currentRateOfFire));
        }
        #endregion

        #region COROUTINES
        IEnumerator SpawnBullet(float delay)
        {
            while (true)
            {
                var projectile = Instantiate(_config.ProjectileScheme.Prefab, transform.position, transform.rotation);
                projectile.transform.SetParent(CortegeController.Instance.ProjectilesContainer);
                projectile.Initialize(_config.ProjectileScheme.Speed, _currentShootDamage, CortegeController.Instance.Limo.transform);

                yield return new WaitForSeconds(delay);
            }
        }
        #endregion
    }
}
