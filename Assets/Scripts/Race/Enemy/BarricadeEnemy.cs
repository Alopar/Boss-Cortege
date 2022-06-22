﻿using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class BarricadeEnemy : AbstractEnemy
    {
        #region FIELDS PRIVATE
        private BarricadeEnemyScheme _scheme;

        private ExplosionComponent _explosion;
        private BarricadeDieComponent _die;
        #endregion

        #region HANDLERS
        private void Explosion_OnExplosion()
        {
            Die();
        }
        #endregion

        #region METHODS PRIVATE
        protected override void Die()
        {
            base.Die();
            _die.Die();

            GameManager.Instance.Wallet.SetCash(_scheme.Money);
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(BarricadeEnemyScheme scheme)
        {
            _scheme = scheme;

            _explosion = GetComponent<ExplosionComponent>();
            _explosion.Init(_scheme.ExplosionDamage);
            _explosion.OnExplosion += Explosion_OnExplosion;

            _die = GetComponent<BarricadeDieComponent>();

            Invoke(nameof(Escape), 3f);
        }
        #endregion
    }
}