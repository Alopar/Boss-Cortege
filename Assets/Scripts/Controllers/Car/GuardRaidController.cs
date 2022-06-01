using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class GuardRaidController : RaidController
    {
        #region FIELDS PRIVATE
        private GuardScheme _config;

        private uint _ramMaxDamage;
        private uint _currentRamDamage;
        #endregion

        #region PROPERTIES
        public GuardScheme Config => _config;
        #endregion

        #region UNITY CALLBACKS
        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);

            var shootEnemy = collision.gameObject.GetComponentInParent<ShootEnemyController>();
            if (shootEnemy != null)
            {
                shootEnemy.SetDamage(_currentRamDamage);
                OnRamWrapper();
            }
        }
        #endregion

        #region METHODS PUBLIC
        public override void Initialize(CarScheme scheme)
        {
            base.Initialize(scheme);

            _config = scheme as GuardScheme;

            _ramMaxDamage = _config.Damage;
            _currentRamDamage = _ramMaxDamage;
        }
        #endregion
    }
}