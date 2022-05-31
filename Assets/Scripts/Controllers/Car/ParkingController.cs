using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public abstract class ParkingController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        protected Place _place;
        protected bool _initialized = false;
        #endregion

        #region PROPERTIES
        public Place Place { get { return _place; } set { _place = value; } }
        #endregion

        #region UNITY CALLBACKS
        private void OnDisable()
        {
            _initialized = false;
        }
        #endregion

        #region METHODS PUBLIC
        public abstract void Initialize(CarScheme scheme, Place place);
        #endregion
    }
}