using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace BossCortege
{
    [SelectionBase]
    public class ShootEnemyController : EnemyController, IDamageable
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _body;
        #endregion

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

        #region EVENTS
        public event Action<uint, int> OnDamage;
        #endregion

        #region UNITY CALLBACKS
        private void Start()
        {
            StartCoroutine(Leave(12f));
        }

        private void FixedUpdate()
        {
            if (IsDie) return;

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
                    base.Die();
                    Destroy(gameObject);
                }
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void SetDamage(uint damage)
        {
            _currentHP -= (int)damage;
            OnDamage?.Invoke(_maxHP, _currentHP);

            if (_currentHP <= 0)
            {
                Die();
            }

            _body.DOShakePosition(0.5f, new Vector3(0.2f, 0, 0), vibrato: 20);
        }

        public override void Die()
        {
            if (IsDie) return;

            base.Die();

            GameManager.Instance.Wallet.SetMoney(_config.Money);

            transform.DOKill();
            StopAllCoroutines();

            _rigidbody.isKinematic = false;

            if(CortegePoint.CortegeColumn == CortegeColumn.One)
            {
                _rigidbody.AddForce(-transform.right * 300f, ForceMode.Impulse);
            }
            else
            {
                _rigidbody.AddForce(transform.right * 300f, ForceMode.Impulse);
            }
            _rigidbody.AddForce(-transform.forward * 1000f, ForceMode.Impulse);
            _rigidbody.AddForce(transform.up * 800f, ForceMode.Impulse);

            var randomTorque = new Vector3(Random.value, Random.value, Random.value);
            _rigidbody.AddTorque(randomTorque * 200f, ForceMode.Impulse);

            Destroy(gameObject, 1.5f);
        }
        #endregion

        #region COROUTINES
        IEnumerator SpawnBullet(float delay)
        {
            while (true)
            {
                var projectile = Instantiate(_config.ProjectileScheme.Prefab, transform.position, transform.rotation);
                projectile.transform.SetParent(RaidManager.Instance.ProjectilesContainer);
                projectile.Initialize(_config.ProjectileScheme.Speed, _currentShootDamage, RaidManager.Instance.Limo.transform);

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

                var row = (CortegeRow)UnityEngine.Random.Range(1, 4);
                _currentPoint = RaidManager.Instance.GetCortegePoint(row, _currentPoint.CortegeColumn);
            }
        }

        IEnumerator Leave(float delay)
        {
            yield return new WaitForSeconds(delay);

            _isLeaving = true;
            _currentSpeed *= 1.5f;
            _currentPoint = RaidManager.Instance.GetCortegePoint(CortegeRow.Front, _currentPoint.CortegeColumn);
        }
        #endregion
    }
}
