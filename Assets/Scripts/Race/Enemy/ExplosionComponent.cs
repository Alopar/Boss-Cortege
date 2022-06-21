using System;
using UnityEngine;

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
            var damageable = collision.gameObject.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.SetDamage(_damage);

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
