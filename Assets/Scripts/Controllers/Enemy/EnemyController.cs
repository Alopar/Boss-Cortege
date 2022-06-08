using System;
using UnityEngine;

namespace BossCortege
{
    public abstract class EnemyController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private bool _isDie = false;
        protected Rigidbody _rigidbody;
        #endregion

        #region PROPERTIES
        public bool IsDie => _isDie;
        public CortegePoint CortegePoint => _currentPoint;
        #endregion

        #region EVENTS
        public event Action<EnemyController> OnEnemyDestroyed;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        #endregion

        #region METHODS PRIVATE
        protected CortegePoint _currentPoint;
        #endregion

        #region METHODS PUBLIC
        public void SetPoint(CortegePoint point)
        {
            _currentPoint = point;
        }

        public virtual void Die()
        {
            _isDie = true;
            OnEnemyDestroyed?.Invoke(this);
        }
        #endregion
    }
}