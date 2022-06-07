using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class LimoRaidController : RaidController, IDamageable
    {
        #region FIELDS PRIVATE
        private LimoScheme _config;
        #endregion

        #region PROPERTIES
        public LimoScheme Config => _config;
        #endregion

        #region EVENTS
        public event Action OnLimoDestroyed;
        #endregion

        #region METHODS PUBLIC
        public override void Initialize(CarScheme scheme)
        {
            base.Initialize(scheme);

            _config = scheme as LimoScheme;
        }

        public override void Die()
        {
            base.Die();
            OnLimoDestroyed?.Invoke();
        }
        #endregion
    }
}