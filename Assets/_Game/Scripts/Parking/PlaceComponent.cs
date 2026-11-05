using System;
using UnityEngine;

namespace BossCortege
{
    public class PlaceComponent : MonoBehaviour, IReplacementable
    {
        #region FIELDS PRIVATE
        private AbstractPlace _place;
        #endregion

        #region EVENTS
        public event Action OnReplaced;
        #endregion

        #region PROPERTIES
        public AbstractPlace Place => _place;
        #endregion

        #region METHODS PUBLIC
        public void SetPlace(AbstractPlace place)
        {
            _place = place;
            ReturnToPlace();
        }

        public AbstractPlace Replace()
        {
            var place = _place;

            _place = null;
            OnReplaced?.Invoke();

            return place;
        }

        public void ReturnToPlace()
        {
            transform.position = _place.SpawnPoint.position;
        }
        #endregion
    }

    public interface IReplacementable
    {
        public event Action OnReplaced;

        public AbstractPlace Replace();
        public void SetPlace(AbstractPlace place);
        public void ReturnToPlace();
    }
}
