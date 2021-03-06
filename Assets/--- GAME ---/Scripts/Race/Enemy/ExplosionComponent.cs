using System;
using UnityEngine;
using Lofelt.NiceVibrations;

namespace BossCortege
{
    public class ExplosionComponent : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private uint _damage;
        #endregion

        #region EVENTS
        public event Action OnExplosion;
        #endregion

        #region UNITY CALLBACKS
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy") return;

            var damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TrySetDamage(_damage, DamageType.Explosion);
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

                OnExplosion?.Invoke();
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
