using System;
using UnityEngine;

namespace BossCortege
{
    public abstract class AbstractCurrencyDeposit
    {
        protected uint _amount;
        protected AbstractStorage<int> _storage;

        public uint Amount => _amount;

        public AbstractCurrencyDeposit(AbstractStorage<int> storage)
        {
            _storage = storage;
        }

        public abstract bool TryGetCash(uint value);
        public abstract void SetCash(uint value);
    }
}