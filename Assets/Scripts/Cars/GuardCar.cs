using System;
using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public class GuardCar : AbstractCar
    {
        #region FIELDS PRIVATE
        private GuardScheme _config;
        #endregion

        #region PROPERTIES
        public GuardScheme Config => _config;
        #endregion

        #region METHODS PUBLIC
        public override void Init(CarScheme scheme, ICarState state)
        {
            base.Init(scheme, state);
            _config = scheme as GuardScheme;
        }
        #endregion
    }
}