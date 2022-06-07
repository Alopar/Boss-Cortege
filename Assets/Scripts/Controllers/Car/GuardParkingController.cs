using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class GuardParkingController : ParkingController
    {
        #region FIELDS PRIVATE
        private GuardScheme _config;
        #endregion

        #region PROPERTIES
        public GuardScheme Config => _config;
        #endregion

        #region METHODS PUBLIC
        public override void Initialize(CarScheme scheme, Place place)
        {
            if (_initialized) return;
            _initialized = true;

            _config = scheme as GuardScheme;
            _place = place;
        }

        public override ParkingController GetCar()
        {
            return this;
        }
        #endregion
    }
}