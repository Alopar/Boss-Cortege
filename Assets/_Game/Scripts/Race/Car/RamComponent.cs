using System;
using UnityEngine;
using BossCortege.EventHolder;
using Lofelt.NiceVibrations;

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
            if (collision.gameObject.tag == "Player") return;

            var damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (damageable.TrySetDamage(_damage, DamageType.Ram))
                {
                    EventHolder<RamInfo>.NotifyListeners(new RamInfo());
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                }
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
