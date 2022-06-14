using System;
using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class DistanceData
    {
        #region FIELDS PRIVATE
        private uint _currentDistance;
        private uint _bestDistance;
        #endregion

        #region PROPERTIES
        public uint CurrentDistance => _currentDistance;
        public uint BestDistance => _bestDistance;
        #endregion

        #region METHODS PUBLIC
        public void SetDistance(uint value)
        {
            _currentDistance = value;
            EventHolder<DistanceChangeInfo>.NotifyListeners(new DistanceChangeInfo(_currentDistance));

            if(_currentDistance > _bestDistance)
            {
                _bestDistance = _currentDistance;
                EventHolder<BestDistanceChangeInfo>.NotifyListeners(new BestDistanceChangeInfo(_bestDistance));
            }
        }
        #endregion
    }
}