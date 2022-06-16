using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public abstract class AbstractCar : MonoBehaviour, IReplacementable
    {
        #region FIELDS PRIVATE
        protected AbstractPlace _place;
        protected bool _initialized = false;
        #endregion

        #region EVENTS
        public event Action OnReplaced;
        #endregion

        #region PROPERTIES
        public AbstractPlace Place => _place;
        #endregion

        #region UNITY CALLBACKS
        private void OnDisable()
        {
            _initialized = false;
        }
        #endregion

        #region METHODS PUBLIC
        public abstract void Initialize(CarScheme scheme);

        public void SetPlace(AbstractPlace place)
        {
            _place = place;
            ReturnToPlace();
        }

        public AbstractPlace Replace()
        {
            var place = _place;

            _place = null;
            OnReplaced?.Invoke();

            return place;
        }

        public void ReturnToPlace()
        {
            transform.position = _place.SpawnPoint.position;
        }
        #endregion
    }
}