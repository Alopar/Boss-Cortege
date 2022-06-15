using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class BossCar : AbstractCar
    {
        #region FIELDS PRIVATE
        private BossScheme _config;
        #endregion

        #region PROPERTIES
        public BossScheme Config => _config;
        #endregion

        #region METHODS PUBLIC
        public override void Initialize(CarScheme scheme, Place place)
        {
            if (_initialized) return;
            _initialized = true;

            _config = scheme as BossScheme;
            _place = place;
        }
        #endregion
    }
}