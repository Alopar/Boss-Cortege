using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    [SelectionBase]
    public class BossCar : AbstractCar
    {
        #region FIELDS PRIVATE
        private BossScheme _config;
        #endregion

        #region PROPERTIES
        public BossScheme Config => _config;
        #endregion

        #region METHODS PUBLIC
        public override void Die()
        {
            base.Die();
            EventHolder<BossDieInfo>.NotifyListeners(new BossDieInfo());
        }

        public override void Init(CarScheme scheme, ICarState state)
        {
            base.Init(scheme, state);
            _config = scheme as BossScheme;
        }
        #endregion
    }
}