using System;
using UnityEngine;

namespace BossCortege
{   
    public abstract class AbstractCar : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _healthPoint;
        [SerializeField] private Transform _smokePoint;
        [SerializeField] private Transform _levelPoint;
        #endregion

        #region PROPERTIES
        public Transform Body => _body;
        public Transform HealthPoint => _healthPoint;
        public Transform SmokePoint => _smokePoint;
        public Transform LevelPoint => _levelPoint;
        #endregion

        #region EVENTS
        public event Action<AbstractCar> OnCarDestroyed;
        #endregion

        #region METHODS PRIVATE
        protected void Escape()
        {
            Die();
            Destroy(gameObject);
        }

        protected virtual void Die()
        {
            OnCarDestroyed?.Invoke(this);
        }
        #endregion

        #region METHODS PUBLIC
        public abstract void Init(CarScheme scheme);
        #endregion
    }
}