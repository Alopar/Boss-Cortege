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

        #region HANDLERS
        private void Health_OnDie()
        {
            throw new NotImplementedException();
        }

        private void Health_OnDamage(uint obj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region METHODS PUBLIC
        public override void Init(CarScheme scheme)
        {
            _config = scheme as GuardScheme;

            var health = GetComponent<HealthComponent>();
            health.OnDamage += Health_OnDamage;
            health.OnDie += Health_OnDie;
        }
        #endregion
    }
}