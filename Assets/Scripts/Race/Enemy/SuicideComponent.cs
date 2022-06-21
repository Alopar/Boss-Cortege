using System;
using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class SuicideComponent : RamComponent
    {
        #region EVENTS
        public event Action OnSuicide;
        #endregion

        #region UNITY CALLBACKS
        private void OnCollisionEnter(Collision collision)
        {
            var damageable = collision.gameObject.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.SetDamage(_damage);

                OnSuicide?.Invoke();
            }
        }
        #endregion
    }
}
