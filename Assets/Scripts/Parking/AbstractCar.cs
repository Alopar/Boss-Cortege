using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public abstract class AbstractCar : MonoBehaviour
    {
        #region FIELDS PRIVATE
        protected PlaceComponent _place;
        #endregion

        #region PROPERTIES
        public PlaceComponent Place => _place;
        #endregion

        #region METHODS PUBLIC
        public abstract void Init(CarScheme scheme);
        #endregion
    }
}