using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class GuardCar : AbstractCar
    {
        #region FIELDS PRIVATE
        private GuardScheme _config;
        #endregion

        #region PROPERTIES
        public GuardScheme Config => _config;
        #endregion

        #region METHODS PUBLIC
        public override void Initialize(CarScheme scheme)
        {
            if (_initialized) return;
            _initialized = true;

            _config = scheme as GuardScheme;
        }
        #endregion
    }
}