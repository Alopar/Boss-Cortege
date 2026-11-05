using System;
using System.Linq;
using System.Collections.Generic;

namespace BossCortege.EventHolder
{
    public static class EventHolder<T> where T : class
    {
        private static readonly List<Action<T>> _listeners = new List<Action<T>>();

        private static T _currentInfo;

        public static void NotifyListeners(T info)
        {
            _currentInfo = info;
            var currentListeners = _listeners.ToList();
            foreach (var listener in currentListeners)
            {
                listener?.Invoke(info);
            }
        }

        public static void AddListener(Action<T> listener, bool instantNotify)
        {
            _listeners.Add(listener);

            if(instantNotify && _currentInfo != null)
            {
                listener?.Invoke(_currentInfo);
            }
        }

        public static void RemoveListener(Action<T> listener)
        {
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }
    }
}