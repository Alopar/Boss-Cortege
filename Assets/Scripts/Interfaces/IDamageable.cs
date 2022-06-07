using System;
using UnityEngine;

namespace BossCortege
{
    public interface IDamageable
    {
        public event Action<uint, int> OnDamage;
        public void SetDamage(uint damage);
    }
}
