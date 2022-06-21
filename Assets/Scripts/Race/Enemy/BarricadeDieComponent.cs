using UnityEngine;

namespace BossCortege
{
    public class BarricadeDieComponent : DieComponent
    {
        #region METHODS PUBLIC
        public override void Die()
        {
            ShowExplosion();
            Destroy(gameObject);
        }
        #endregion
    }
}
