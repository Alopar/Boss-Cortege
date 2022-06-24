using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class HealthComponent : MonoBehaviour, IDamageable, IHealthable
    {
        #region FIELDS PRIVATE
        private uint _maxHP;
        private int _currentHP;
        private bool _isInvulnerability = false;
        #endregion

        #region EVENTS
        public event Action<uint, int> OnChangeHP;
        public event Action<uint> OnDamage;
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
            OnChangeHP?.Invoke(_maxHP, _currentHP);
        }

        public bool TrySetDamage(uint damage)
        {
            if (_isInvulnerability) return false;

            _currentHP -= (int)damage;
            StartCoroutine(InvulnerabilityTimer(0.5f));

            OnDamage?.Invoke(damage);
            OnChangeHP?.Invoke(_maxHP, _currentHP);

            if (_currentHP <= 0)
            {
                OnDie?.Invoke();
            }

            return true;
        }
        #endregion

        #region COROUTINES
        IEnumerator InvulnerabilityTimer(float delay)
        {
            _isInvulnerability = true;
            yield return new WaitForSeconds(delay);
            _isInvulnerability = false;
        }
        #endregion
    }

    public interface IDamageable
    {
        public event Action<uint> OnDamage;
        public bool TrySetDamage(uint damage);
    }

    public interface IHealthable
    {
        public void AddHealth(uint value);
    }
}
