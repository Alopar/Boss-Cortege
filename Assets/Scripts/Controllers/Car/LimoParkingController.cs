using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class LimoParkingController : ParkingController
    {
        #region FIELDS PRIVATE
        private LimoScheme _config;
        #endregion

        #region PROPERTIES
        public LimoScheme Config => _config;
        #endregion

        #region METHODS PUBLIC
        public override void Initialize(CarScheme scheme, Place place)
        {
            if (_initialized) return;
            _initialized = true;

            _config = scheme as LimoScheme;
            _place = place;
        }

        public override ParkingController GetCar()
        {
            return this;
        }
        #endregion
    }
}