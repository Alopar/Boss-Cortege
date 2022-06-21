using System;
using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public abstract class AbstractEnemy : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private bool _isDie = false;
        #endregion

        #region PROPERTIES
        public bool IsDie => _isDie;
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
            _isDie = true;
            OnEnemyDestroyed?.Invoke(this);
        }
        #endregion
    }
}