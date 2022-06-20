using System;
using UnityEngine;

namespace BossCortege
{
    public class HealthComponent : MonoBehaviour, IDamageable, IHealthable
    {
        #region FIELDS PRIVATE
        private uint _maxHP;
        private int _currentHP;
        #endregion

        #region EVENTS
        public event Action<uint, int> OnDamage;
        public event Action OnDie;
        #endregion

        #region METHODS PUBLIC
        public void Init(uint maxHP)
        {
            _maxHP = maxHP;
            _currentHP = (int)_maxHP;
        }

        public void AddHealth(uint value)
        {
            _currentHP = Mathf.Clamp(_currentHP + (int)value, 0, (int)_maxHP);
            OnDamage?.Invoke(_maxHP, _currentHP);
        }

        public void SetDamage(uint damage)
        {
            _currentHP -= (int)damage;
            OnDamage?.Invoke(_maxHP, _currentHP);

            if (_currentHP <= 0)
            {
                OnDie?.Invoke();
            }
        }
        #endregion
    }

    public interface IDamageable
    {
        public event Action<uint, int> OnDamage;
        public void SetDamage(uint damage);
    }

    public interface IHealthable
    {
        public void AddHealth(uint value);
    }
}
