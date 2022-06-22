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
        public override void SetScheme(CarScheme scheme)
        {
            _config = scheme as GuardScheme;
        }
        #endregion
    }
}