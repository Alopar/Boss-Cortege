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
            UnlockPlace();
        }
        #endregion

        #region METHODS PRIVATE
        public void UnlockPlace()
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
        public void AddPlace()
        {
            _currentAvailableParkingPlaces++;
            UnlockPlace();
        }
        #endregion
    }
}
