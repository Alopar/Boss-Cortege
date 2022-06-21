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
            base.Die();
            _die.Die();

            GameManager.Instance.Wallet.SetCash(_scheme.Money);
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(SuicideEnemyScheme scheme)
        {
            _scheme = scheme;
            
            _suicide = GetComponent<SuicideComponent>();
            _suicide.Init(_scheme.RamDamage);
            _suicide.OnSuicide += Suicide_OnSuicide;

            _move = GetComponent<MoveComponent>();
            _move.OnPointReached += Move_OnPointReached;

            _die = GetComponent<SuicideDieComponent>();
        }
        #endregion
    }
}