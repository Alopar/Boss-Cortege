using System;
using UnityEngine;

namespace BossCortege
{
    public class RaidController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private bool _isLimo;
        private Car _settings;

        private float _speed;

        private int _maxHP;
        private int _currentHP;

        private uint _maxDamage;
        private uint _currentDamage;

        private Collider _collider;
        private Rigidbody _rigidbody;

        private CortegePoint _currentPoint;
        private CortegePoint _previousPoint;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Only the initial installation is available
        /// </summary>
        public Car Settings
        {
            get { return _settings; }
            set
            {
                if (_settings == null)
                {
                    _settings = value;

                    _maxHP = (int)_settings.Durability;
                    _currentHP = _maxHP;

                    _maxDamage = _settings.Damage;
                    _currentDamage = _maxDamage;
                }
            }
        }
        public CortegePoint CortegePoint => _currentPoint;
        public bool IsLimo { get { return _isLimo; } set { _isLimo = value; } }
        public float Speed { get { return _speed; } set { _speed = value; } }
        #endregion

        #region EVENTS
        public event Action<RaidController> OnRaidDestroyed;
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _collider = GetComponentInChildren<BoxCollider>();
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var suicideEnemy = collision.gameObject.GetComponentInParent<SuicideController>();
            if(suicideEnemy != null)
            {
                SetDamage(suicideEnemy.Damage);
                suicideEnemy.Die();
            }

            var bulletEnemy = collision.gameObject.GetComponentInParent<BulletController>();
            if (bulletEnemy != null)
            {
                bulletEnemy.Die();
            }

            var bullet = collision.gameObject.GetComponent<Bullet>();
            if(bullet != null)
            {
                SetDamage(bullet.Damage);
                Destroy(bullet.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Gate")
            {
                if (_isLimo)
                {
                    GameManager.Instance.AddMoney();
                    GameManager.Instance.StopCortege();
                }
            }
        }
        #endregion

        #region METHODS PRIVATE
        #endregion

        #region METHODS PUBLIC
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

        public void Die()
        {
            OnRaidDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        #endregion
    }
}
