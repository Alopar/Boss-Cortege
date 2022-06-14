using System;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class Wallet
    {
        #region FIELDS PRIVATE
        private uint _money;
        #endregion

        #region PROPERTIES
        public uint Money => _money;
        #endregion

        #region METHODS PUBLIC
        public bool TryGetMoney(uint value)
        {
            if (_money >= value)
            {
                _money -= value;
                EventHolder<MoneyChangeInfo>.NotifyListeners(new MoneyChangeInfo(_money));

                return true;
            }

            return false;
        }

        public void SetMoney(uint value)
        {
            _money += value;
            EventHolder<MoneyChangeInfo>.NotifyListeners(new MoneyChangeInfo(_money));
        }
        #endregion
    }
}