using UnityEngine;

namespace BossCortege
{
    public class Place : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _spawnPoint;
        #endregion

        #region FIELDS PRIVATE
        private CarController _car;
        #endregion

        #region PROPERTIES
        public CarController Car => _car;
        #endregion


        #region METHODS PUBLIC
        public void PlaceCar(CarController car)
        {
            _car = car;
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
