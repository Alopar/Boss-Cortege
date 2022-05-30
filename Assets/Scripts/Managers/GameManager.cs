using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class GameManager : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _parkingCamera;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _cortegeCamera;
        #endregion

        #region FIELDS PRIVATE
        private static GameManager _instance;

        private int _money;
        private List<CortegePlace> _cortegePlace;
        private List<ParkingPlace> _parkingPlace;
        private CortegeController _cortege;
        #endregion

        #region PROPERTIES
        public static GameManager Instance => _instance;

        public int Money => _money;
        #endregion

        #region EVENTS
        public event Action OnCortegeStop;
        public event Action<int> OnMoneyChange;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            _cortege = FindObjectOfType<CortegeController>();
            _cortegePlace = FindObjectsOfType<CortegePlace>().ToList();
            _parkingPlace = FindObjectsOfType<ParkingPlace>().OrderBy(e => e.Number).ToList();

            var limoPlace = _cortegePlace.Find(e => e.IsLimo);
            var limoScheme = Resources.Load<Car>("Limo");
            SpawnCar(limoScheme, limoPlace);

            SetMoney(1000);
        }
        #endregion

        #region METHODS PRIVATE
        private void SpawnCar(Car carScheme, Place place)
        {
            var car = Instantiate(carScheme.CarPrefab);
            car.Settings = carScheme;
            place.PlaceCar(car);
        }

        private void SetMoney(uint coins)
        {
            _money += (int)coins;
            OnMoneyChange?.Invoke(_money);
        }

        private bool GetMoney(uint coins)
        {
            if(_money >= coins)
            {
                _money -= (int)coins;
                OnMoneyChange?.Invoke(_money);

                return true;
            }

            return false;
        }
        #endregion

        #region METHODS PUBLIC
        public void BuyCar()
        {
            foreach (var place in _parkingPlace)
            {
                if(place.Car == null)
                {
                    var baseCarScheme = Resources.Load<Car>("Car01");
                    if (GetMoney(baseCarScheme.Cost))
                    {
                        SpawnCar(baseCarScheme, place);
                    }

                    return;
                }
            }
        }

        public void MergeCar(CarController dominantCar, CarController submissiveCar)
        {
            string carSchemeName;
            switch (dominantCar.Settings.Level)
            {
                case CarLevel.Level01:
                    carSchemeName = "Car02";
                    break;
                case CarLevel.Level02:
                    carSchemeName = "Car03";
                    break;
                case CarLevel.Level03:
                    carSchemeName = "Car04";
                    break;
                case CarLevel.Level04:
                    carSchemeName = "Car05";
                    break;
                case CarLevel.Level05:
                    carSchemeName = "Car05";
                    break;
                default:
                    carSchemeName = "Car05";
                    break;
            }
            var nextLevelCarScheme = Resources.Load<Car>(carSchemeName);

            SpawnCar(nextLevelCarScheme, submissiveCar.Place);

            dominantCar.Place.ClearPlace();

            Destroy(dominantCar.gameObject);
            Destroy(submissiveCar.gameObject);
        }

        public void SwapCar(CarController dominantCar, CarController submissiveCar)
        {
            var tempPlace = submissiveCar.Place;
            dominantCar.Place.PlaceCar(submissiveCar);
            tempPlace.PlaceCar(dominantCar);            
        }

        public void GoCortege()
        {
            var predicates = new List<Predicate<CortegePlace>>();
            predicates.Add(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Two);
            predicates.Add(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Three);
            predicates.Add(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Four);
            predicates.Add(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Two);
            predicates.Add(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Three);
            predicates.Add(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Four);
            predicates.Add(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Two);
            predicates.Add(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Three);
            predicates.Add(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Four);

            foreach (var p in predicates)
            {
                var place = _cortegePlace.Find(p);
                if (place.Car != null)
                {
                    place.Car.gameObject.SetActive(false);
                    _cortege.SetCar(place.CortegeRow, place.CortegeColumn, place.Car);
                }
            }

            _cortege.Go();
            _cortegeCamera.Priority = 30;
        }

        public void StopCortege()
        {
            _cortege.Stop();
            _cortegeCamera.Priority = 10;

            foreach (var place in _cortegePlace)
            {
                place.Car?.gameObject.SetActive(true);
            }

            OnCortegeStop?.Invoke();
        }

        public void AddMoney()
        {
            SetMoney(500);
        }

#if UNITY_EDITOR
        [ContextMenu("cheat: MoreMoney")]
        public void MoreMoney()
        {
            SetMoney(1000);
        }
#endif
        #endregion

    }
}