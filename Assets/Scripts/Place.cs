using UnityEngine;

namespace BossCortege
{
    public class Place : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _spawnPoint;
        #endregion

        #region FIELDS PRIVATE
        private ParkingController _car;
        #endregion

        #region PROPERTIES
        public ParkingController Car => _car;
        #endregion

        #region METHODS PUBLIC
        public void PlaceCar(ParkingController parking)
        {
            _car = parking;
            _car.Place = this;
            _car.transform.position = _spawnPoint.position;
        }

        public void ClearPlace()
        {
            _car = null;
        }
        #endregion
    }
}
