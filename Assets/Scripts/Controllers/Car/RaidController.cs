using System;
using UnityEngine;

namespace BossCortege
{
    public abstract class RaidController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private float _speed;

        private uint _maxHP;
        private int _currentHP;

        private CortegePoint _currentPoint;
        private CortegePoint _previousPoint;

        private bool _initialized = false;
        #endregion

        #region PROPERTIES
        public CortegePoint CortegePoint => _currentPoint;
        public float Speed { get { return _speed; } set { _speed = value; } }
        #endregion

        #region EVENTS
        public event Action<RaidController> OnRaidDestroyed;
        #endregion

        #region UNITY CALLBACKS
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var suicideEnemy = collision.gameObject.GetComponentInParent<SuicideEnemyController>();
            if(suicideEnemy != null)
            {
                SetDamage(suicideEnemy.RamDamage);
                suicideEnemy.Die();

                GameManager.Instance.SetMoney(suicideEnemy.Config.Money);
            }

            var shootEnemy = collision.gameObject.GetComponentInParent<ShootEnemyController>();
            if (shootEnemy != null)
            {
                shootEnemy.Die();

                GameManager.Instance.SetMoney(shootEnemy.Config.Money);
            }

            var projectile = collision.gameObject.GetComponent<ProjectileController>();
            if(projectile != null)
            {
                SetDamage(projectile.Damage);
                Destroy(projectile.gameObject);
            }
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
            if(_currentHP <= 0)
            {
                Die();
            }
        }

        public void SetPoint(CortegePoint point)
        {
            _previousPoint = _currentPoint;
            _currentPoint = point;
        }

        public virtual void Die()
        {
            OnRaidDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        #endregion
    }
}
