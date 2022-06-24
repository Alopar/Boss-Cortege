using System;
using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public abstract class AbstractEnemy : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _healthPoint;
        [SerializeField] private Transform _smokePoint;
        #endregion

        #region PROPERTIES
        public Transform Body => _body;
        public Transform HealthPoint => _healthPoint;
        public Transform SmokePoint => _smokePoint;
        #endregion

        #region EVENTS
        public event Action<AbstractEnemy> OnEnemyDestroyed;
        #endregion

        #region METHODS PRIVATE
        protected void Escape()
        {
            Die();
            Destroy(gameObject);
        }

        protected virtual void Die()
        {
            OnEnemyDestroyed?.Invoke(this);
        }
        #endregion

        #region METHODS PUBLIC
        public abstract void Init(EnemyScheme scheme);
        #endregion
    }
}