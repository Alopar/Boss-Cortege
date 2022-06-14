using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public abstract class ParkingController : MonoBehaviour, IReplacementable
    {
        #region FIELDS PRIVATE
        protected Place _place;
        protected bool _initialized = false;
        #endregion

        #region EVENTS
        public event Action OnReplaced;
        #endregion


        #region PROPERTIES
        public Place Place => _place;
        #endregion

        #region UNITY CALLBACKS
        private void OnDisable()
        {
            _initialized = false;
        }
        #endregion

        #region METHODS PUBLIC
        public abstract void Initialize(CarScheme scheme, Place place);

        public void SetPlace(Place place)
        {
            _place = place;
            ReturnToPlace();
        }

        public Place Replace()
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

        public abstract ParkingController GetCar();
        #endregion
    }
}