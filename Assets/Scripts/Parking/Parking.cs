using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class Parking : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private List<ParkingPlace> _parkingPlaces;
        [SerializeField] private uint _baseAvailableParkingPlaces = 3;
        #endregion

        #region FIELDS PRIVATE
        private uint _currentAvailableParkingPlaces;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _currentAvailableParkingPlaces = _baseAvailableParkingPlaces;
        }

        private void Start()
        {
            Init();
        }
        #endregion

        #region METHODS PRIVATE
        private void UnlockPlaces()
        {
            foreach (var place in _parkingPlaces)
            {
                if (place.Number <= _currentAvailableParkingPlaces)
                {
                    place.Unlock();
                }
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void Init()
        {
            UnlockPlaces();
        }

        public void AddPlace()
        {
            _currentAvailableParkingPlaces++;
            UnlockPlaces();
        }
        #endregion
    }
}
