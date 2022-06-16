using System;
using UnityEngine;
using DG.Tweening;

namespace BossCortege
{
    public abstract class RaidController : MonoBehaviour, IDamageable
    {
        #region FIELDS PRIVATE
        private float _speed;

        private uint _maxHP;
        private int _currentHP;

        private RacePoint _currentPoint;

        private bool _initialized = false;
        #endregion

        #region PROPERTIES
        public RacePoint CortegePoint => _currentPoint;
        public float Speed { get { return _speed; } set { _speed = value; } }
        #endregion

        #region EVENTS
        public event Action<RaidController> OnRaidDestroyed;
        public event Action<uint, int> OnDamage;
        public event Action OnRam;
        #endregion

        #region UNITY CALLBACKS
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.deltaTime);

            if(transform.position == _currentPoint.transform.position)
            {
                transform.DORotate(new Vector3(0, 0, 0), 0.1f);
                RaceManager.Instance.DropAttack();
            }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            var projectile = collision.gameObject.GetComponent<ProjectileController>();
            if (projectile != null)
            {
                SetDamage(projectile.Damage);
                Destroy(projectile.gameObject);
            }

            var suicideEnemy = collision.gameObject.GetComponentInParent<SuicideEnemyController>();
            if(suicideEnemy != null)
            {
                if (!suicideEnemy.IsDie)
                {
                    SetDamage(suicideEnemy.RamDamage);
                    suicideEnemy.Die();

                    GameManager.Instance.Wallet.SetCash(suicideEnemy.Config.Money);
                }
            }

            var barricadeEnemy = collision.gameObject.GetComponentInParent<BarricadeEnemyController>();
            if (barricadeEnemy != null)
            {
                SetDamage(barricadeEnemy.ExplosionDamage);
                barricadeEnemy.Die();

                GameManager.Instance.Wallet.SetCash(barricadeEnemy.Config.Money);
            }
        }
        #endregion

        #region METHODS PRIVATE
        protected void OnRamWrapper()
        {
            OnRam?.Invoke();
        }
        #endregion

        #region METHODS PUBLIC
        public virtual void Initialize(CarScheme scheme)
        {
            if (_initialized) return;
            _initialized = true;

            _maxHP = scheme.Durability;
            _currentHP = (int)_maxHP;
        }

        public void SetDamage(uint damage)
        {
            _currentHP -= (int)damage;
            OnDamage?.Invoke(_maxHP, _currentHP);

            if(_currentHP <= 0)
            {
                Die();
            }
        }

        public void SetPoint(RacePoint point)
        {
            if(_currentPoint != null && _currentPoint.RaidController == this)
            {
                _currentPoint.RaidController = null;
            }

            if(_currentPoint != null)
            {
                if (_currentPoint.transform.position.x > point.transform.position.x)
                {
                    transform.DORotate(new Vector3(0, -7, 0), 0.2f);
                }
                else
                {
                    transform.DORotate(new Vector3(0, 7, 0), 0.2f);
                }
            }

            _currentPoint = point;
            _currentPoint.RaidController = this;
        }

        public virtual void Die()
        {
            OnRaidDestroyed?.Invoke(this);
        }
        #endregion
    }
}
