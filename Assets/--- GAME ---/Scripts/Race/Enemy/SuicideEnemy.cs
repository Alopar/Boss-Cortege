using BossCortege.EventHolder;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class SuicideEnemy : AbstractEnemy
    {
        #region FIELDS PRIVATE
        private SuicideEnemyScheme _scheme;

        private SuicideComponent _suicide;
        private SuicideDieComponent _die;
        private MoveComponent _move;
        #endregion

        #region PROPERTIES
        public MoveComponent Move => _move;
        #endregion

        #region HANDLERS
        private void Suicide_OnSuicide()
        {
            Die();
        }

        private void Move_OnPointReached()
        {
            Escape();
        }
        #endregion

        #region METHODS PRIVATE
        protected override void Die()
        {
            GameObject.Destroy(_move);
            GameObject.Destroy(_suicide);

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
            _scheme = scheme as SuicideEnemyScheme;
            
            _suicide = GetComponent<SuicideComponent>();
            _suicide.OnSuicide += Suicide_OnSuicide;

            _move = GetComponent<MoveComponent>();
            _move.OnPointReached += Move_OnPointReached;

            _die = GetComponent<SuicideDieComponent>();
        }
        #endregion
    }
}