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

        private uint _maxHP;
        private int  _currentHP;

        private uint _shootMaxDamage;
        private uint _currentShootDamage;

        private Coroutine _fireCoroutine = null;
        private Coroutine _moveNextCoroutine = null;

        private bool _isLeaving = false;
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

                    _maxHP = _config.Durability;
                    _currentHP = (int)_maxHP;

                    _currentSpeed = _config.Speed;
                    _currentRateOfFire = _config.RateOfFire;
                    
                    _shootMaxDamage = _config.ShootDamage;
                    _currentShootDamage = _shootMaxDamage;
                }
            }
        }
        #endregion

        #region UNITY CALLBACKS
        private void Start()
        {
            StartCoroutine(Leave(12f));
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _currentSpeed * Time.fixedDeltaTime);

            if (transform.position == _currentPoint.transform.position)
            {
                if (_fireCoroutine == null)
                {
                    _fireCoroutine = StartCoroutine(SpawnBullet(_currentRateOfFire));
                }

                if (_moveNextCoroutine == null)
                {
                    _moveNextCoroutine = StartCoroutine(MoveNextRow(3f));
                }

                if (_isLeaving)
                {
                    Die();
                }
            }
        }
        #endregion

        #region METHODS PRIVATE
        public override void Die()
        {
            base.Die();
            GameManager.Instance.SetMoney(_config.Money);
        }
        #endregion

        #region METHODS PUBLIC
        public void SetDamage(uint damage)
        {
            _currentHP -= (int)damage;
            if (_currentHP <= 0)
            {
                Die();
            }
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

                if (_isLeaving) break;
            }
        }

        IEnumerator MoveNextRow(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);

                if (_isLeaving) break;

                var row = (CortegeRow)Random.Range(1, 4);
                _currentPoint = CortegeController.Instance.GetCortegePoint(row, _currentPoint.CortegeColumn);
            }
        }

        IEnumerator Leave(float delay)
        {
            yield return new WaitForSeconds(delay);

            _isLeaving = true;
            _currentPoint = CortegeController.Instance.GetCortegePoint(CortegeRow.Front, _currentPoint.CortegeColumn);
        }
        #endregion
    }
}
