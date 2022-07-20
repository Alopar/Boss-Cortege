using System;
using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class DistanceHolder
    {
        private AbstractStorage<int> _storage;

        private uint _currentDistance;
        private uint _bestDistance;

        public uint CurrentDistance => _currentDistance;
        public uint BestDistance => _bestDistance;

        public DistanceHolder(AbstractStorage<int> storage)
        {
            _storage = storage;
            _bestDistance = (uint)_storage.Load();
        }

        public void SetDistance(uint value)
        {
            _currentDistance = value;
            EventHolder<DistanceChangeInfo>.NotifyListeners(new DistanceChangeInfo(_currentDistance));

            if(_currentDistance > _bestDistance)
            {
                _bestDistance = _currentDistance;
                _storage.Save((int)_bestDistance);
                EventHolder<BestDistanceChangeInfo>.NotifyListeners(new BestDistanceChangeInfo(_bestDistance));
            }
        }
    }
}