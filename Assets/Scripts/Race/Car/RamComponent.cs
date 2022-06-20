using System;
using UnityEngine;

namespace BossCortege
{
    public class RamComponent : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private uint _ramMaxDamage;
        private uint _currentRamDamage;
        #endregion

        #region EVENTS
        public event Action OnRam;
        #endregion

        #region UNITY CALLBACKS
        protected void OnCollisionEnter(Collision collision)
        {
            var damageable = collision.gameObject.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.SetDamage(_currentRamDamage);
                OnRam?.Invoke();
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(uint damage)
        {
            _ramMaxDamage = damage;
            _currentRamDamage = _ramMaxDamage;
        }
        #endregion
    }
}
