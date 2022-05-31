using System;
using UnityEngine;

namespace BossCortege
{
    public abstract class EnemyController : MonoBehaviour
    {
        #region METHODS PRIVATE
        protected CortegePoint _currentPoint;
        #endregion

        #region PROPERTIES
        public CortegePoint CortegePoint => _currentPoint;
        #endregion

        #region EVENTS
        public event Action<EnemyController> OnEnemyDestroyed;
        #endregion

        #region METHODS PUBLIC
        public void SetPoint(CortegePoint point)
        {
            _currentPoint = point;
        }

        public void Die()
        {
            OnEnemyDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        #endregion
    }
}