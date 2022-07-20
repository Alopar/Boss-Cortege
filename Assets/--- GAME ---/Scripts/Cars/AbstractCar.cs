using System;
using UnityEngine;
using DG.Tweening;

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

        #region FIELDS PRIVATE
        private ICarState _state;
        #endregion

        #region PROPERTIES
        public Transform Body => _body;
        public Transform HealthPoint => _healthPoint;
        public Transform SmokePoint => _smokePoint;
        public Transform LevelPoint => _levelPoint;
        public ICarState State => _state;
        #endregion

        #region EVENTS
        public event Action<AbstractCar> OnCarDestroyed;
        #endregion

        #region METHODS PUBLIC
        public void Escape()
        {
            Die();
            Destroy(gameObject);
        }

        public virtual void Die()
        {
            _body.DOKill();
            OnCarDestroyed?.Invoke(this);
        }

        public virtual void Init(CarScheme scheme, ICarState state)
        {
            _state = state;
            _state.Init(this);
        }
        #endregion
    }
}