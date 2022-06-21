using System;
using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class RamComponent : MonoBehaviour
    {
        #region FIELDS PRIVATE
        protected uint _damage;
        #endregion

        #region UNITY CALLBACKS
        private void OnCollisionEnter(Collision collision)
        {
            var damageable = collision.gameObject.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.SetDamage(_damage);

                EventHolder<RamInfo>.NotifyListeners(new RamInfo());
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(uint damage)
        {
            _damage = damage;
        }
        #endregion
    }
}
