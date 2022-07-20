using BossCortege.EventHolder;
using System.Collections;
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

            EventHolder<EnemyDieInfo>.NotifyListeners(new EnemyDieInfo(_scheme.Money));

            var popupPrefab = Resources.Load<PopupInfo>("PopupInfos/Coins");
            PopupInfoManager.Instance.ShowPopupInfoText(popupPrefab, Camera.main.WorldToScreenPoint(transform.position), _scheme.Money.ToString());
        }
        #endregion

        #region METHODS PUBLIC
        public override void Init(EnemyScheme scheme)
        {
            _scheme = scheme as BarricadeEnemyScheme;

            _explosion = GetComponent<ExplosionComponent>();
            _explosion.OnExplosion += Explosion_OnExplosion;

            _die = GetComponent<BarricadeDieComponent>();

            Invoke(nameof(Escape), 5f);
        }
        #endregion
    }
}