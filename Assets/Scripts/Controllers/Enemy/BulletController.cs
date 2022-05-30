using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public class BulletController : EnemyController
    {
        #region FIELDS PRIVATE
        private BulletEnemy _settings;

        private float _speed;
        private float _rateOfFire;

        private uint _maxDamage;
        private uint _currentDamage;

        private bool _isFire = false;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Only the initial installation is available
        /// </summary>
        public BulletEnemy Settings
        {
            get { return _settings; }
            set
            {
                if (_settings == null)
                {
                    _settings = value;

                    _speed = _settings.Speed;
                    _rateOfFire = _settings.RateOfFire;
                    
                    _maxDamage = _settings.Damage;
                    _currentDamage = _maxDamage;
                }
            }
        }

        #endregion

        #region UNITY CALLBACKS
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.deltaTime);

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
            StartCoroutine(SpawnBullet(_rateOfFire));
        }
        #endregion

        #region METHODS PUBLIC
        public void SetPoint(CortegePoint point)
        {
            _currentPoint = point;
        }
        #endregion

        #region COROUTINES
        IEnumerator SpawnBullet(float delay)
        {
            while (true)
            {
                var bullet = Instantiate(_settings.BulletPrefab, transform.position, transform.rotation);
                bullet.Damage = _currentDamage;
                bullet.Speed = _settings.BulletSpeed;
                yield return new WaitForSeconds(delay);
            }
        }
        #endregion
    }
}
