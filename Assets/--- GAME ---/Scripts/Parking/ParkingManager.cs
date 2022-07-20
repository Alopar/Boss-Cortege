using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class ParkingManager : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private uint _baseAvailableParkingPlaces = 3;

        [Space(10)]
        [SerializeField] private BarrieController _barrieController;

        [Space(10)]
        [SerializeField] private GameObject _spawnCarVFX;
        [SerializeField] private GameObject _mergeCarVFX;
        #endregion

        #region FIELDS PRIVATE
        private AbstractStorage<int> _availableParkingPlaceStorage;
        private AbstractStorage<string> _carToParkingStorage;
        private AbstractStorage<string> _carToCortegeStorage;

        private List<CortegePlace> _cortegePlaces;
        private List<ParkingPlace> _parkingPlaces;

        private uint _currentAvailableParkingPlaces;

        private static ParkingManager _instance;
        #endregion

        #region PROPERTIES
        public static ParkingManager Instance => _instance;
        #endregion

        #region HANDLERS
        private void BuyCarHandler(BuyCarInfo info)
        {
            foreach (var place in _parkingPlaces)
            {
                if (place.IsVacant && place.IsEmpty)
                {
                    if (GameManager.Instance.Wallet.TryGetCash(info.Cost))
                    {
                        SpawnCar(new BuildParkingGuard(PowerLevel.Level01), place);
                        Instantiate(_spawnCarVFX, place.SpawnPoint.position, Quaternion.identity);

                        SaveCarsOnPlaces();
                    }

                    return;
                }
            }
        }

        private void BuyPlaceHandler(BuyPlaceInfo info)
        {
            if (GameManager.Instance.Wallet.TryGetCash(info.Cost))
            {
                _currentAvailableParkingPlaces++;
                _availableParkingPlaceStorage.Save((int)_currentAvailableParkingPlaces);
                UnlockPlaces();
            }
        }

        private void RaceStartHandler(RaceStartInfo info)
        {
            UpBarrier();
            HideCortegeCars();
        }

        private void RaceStopHandler(RaceStopInfo info)
        {
            DownBarrier();
            ShowCortegeCars();
        }

        private void MergeCarHandler(MergeCarInfo info)
        {
            var dominantCar = info.FirstCar;
            var dominantPlace = info.FirstPlace;

            var submissiveCar = info.SecondCar;
            var submissivePlace = info.SecondPlace;

            var nextLevel = (PowerLevel)Mathf.Clamp((int)dominantCar.Config.Level + 1, 1, 18);
            var place = submissivePlace.Replace();

            dominantPlace.Replace();
            Destroy(dominantCar.gameObject);
            Destroy(submissiveCar.gameObject);

            SpawnCar(new BuildParkingGuard(nextLevel), place);
            Instantiate(_mergeCarVFX, place.SpawnPoint.position, Quaternion.identity);

            SaveCarsOnPlaces();
        }

        private void SwapCarHandler(SwapCarInfo info)
        {
            var dominantPlaceComponent = info.FirstPlace;
            var submissivePlaceComponent = info.SecondPlace;

            var dominantCarPlace = dominantPlaceComponent.Replace();
            var submissiveCarPlace = submissivePlaceComponent.Replace();

            dominantCarPlace.PlaceVechicle(submissivePlaceComponent);
            submissiveCarPlace.PlaceVechicle(dominantPlaceComponent);

            SaveCarsOnPlaces();
        }

        private void ReplaceCarHandler(ReplaceCarInfo info)
        {
            SaveCarsOnPlaces();
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<BuyCarInfo>.AddListener(BuyCarHandler, false);
            EventHolder<BuyPlaceInfo>.AddListener(BuyPlaceHandler, false);
            EventHolder<RaceStartInfo>.AddListener(RaceStartHandler, false);
            EventHolder<RaceStopInfo>.AddListener(RaceStopHandler, false);
            EventHolder<MergeCarInfo>.AddListener(MergeCarHandler, false);
            EventHolder<SwapCarInfo>.AddListener(SwapCarHandler, false);
            EventHolder<ReplaceCarInfo>.AddListener(ReplaceCarHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<BuyCarInfo>.RemoveListener(BuyCarHandler);
            EventHolder<BuyPlaceInfo>.RemoveListener(BuyPlaceHandler);
            EventHolder<RaceStartInfo>.RemoveListener(RaceStartHandler);
            EventHolder<RaceStopInfo>.RemoveListener(RaceStopHandler);
            EventHolder<MergeCarInfo>.RemoveListener(MergeCarHandler);
            EventHolder<SwapCarInfo>.RemoveListener(SwapCarHandler);
            EventHolder<ReplaceCarInfo>.RemoveListener(ReplaceCarHandler);
        }

        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Init();
        }
        #endregion

        #region METHODS PRIVATE
        private void Init()
        {   
            _availableParkingPlaceStorage = new IntPlayerPrefStorage("AVAILABLE-PARKING-PLACES");
            _currentAvailableParkingPlaces = (uint)_availableParkingPlaceStorage.Load();

            if(_currentAvailableParkingPlaces == 0)
            {
                _currentAvailableParkingPlaces = _baseAvailableParkingPlaces;
                _availableParkingPlaceStorage.Save((int)_currentAvailableParkingPlaces);
            }

            _cortegePlaces = FindObjectsOfType<CortegePlace>().ToList();
            _parkingPlaces = FindObjectsOfType<ParkingPlace>().OrderBy(e => e.Number).ToList();

            UnlockPlaces();

            var bossPlace = _cortegePlaces.Find(e => e.IsBoss);
            SpawnCar(new BuildParkingBoss(), bossPlace);

            _carToParkingStorage = new StringPlayerPrefStorage("CARS-ON-PARKING");
            _carToCortegeStorage = new StringPlayerPrefStorage("CARS-ON-CORTEGE");

            RestoreCarsOnParking();
            RestoreCarsOnCortege();
        }

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

        private void SpawnCar(IBuildCarStrategy builder, AbstractPlace place)
        {
            var car = builder.BuildCar();
            var replacement = car.GetComponent<IReplacementable>();
            place.PlaceVechicle(replacement);
        }

        private void ShowCortegeCars()
        {
            var cortegeCars = GetCortegeCars();
            cortegeCars.ForEach(e => e.gameObject.SetActive(true));
        }

        private void HideCortegeCars()
        {
            var cortegeCars = GetCortegeCars();
            cortegeCars.ForEach(e => e.gameObject.SetActive(false));
        }

        private void UpBarrier()
        {
            _barrieController.UpBarrier();
        }

        private void DownBarrier()
        {
            _barrieController.DownBarrier();
        }

        private void SaveCarsOnPlaces()
        {
            SaveCarsOnParking();
            SaveCarsOnCortege();
        }

        private void SaveCarsOnParking()
        {
            var parkingCars = FindObjectsOfType<GuardCar>().ToList();
            parkingCars = parkingCars.FindAll(e => e.TryGetComponent<PlaceComponent>(out PlaceComponent component) && component.Place != null && component.Place is ParkingPlace);

            var dataJSON = new ListDataJSON<CarOnParking>() { data = new List<CarOnParking>() };
            foreach (var parkingCar in parkingCars)
            {
                var carToParking = new CarOnParking();
                carToParking.powerLevel = (int)parkingCar.Config.Level;
                carToParking.placeNumber = (int)(parkingCar.GetComponent<PlaceComponent>().Place as ParkingPlace).Number;

                dataJSON.data.Add(carToParking);
            }

            var stringDataJSON = JsonUtility.ToJson(dataJSON);
            _carToParkingStorage.Save(stringDataJSON);
        }

        private void SaveCarsOnCortege()
        {
            var cortegeCars = FindObjectsOfType<GuardCar>().ToList();
            cortegeCars = cortegeCars.FindAll(e => e.TryGetComponent<PlaceComponent>(out PlaceComponent component) && component.Place != null && component.Place is CortegePlace);

            var dataJSON = new ListDataJSON<CarOnCortege>() { data = new List<CarOnCortege>() };
            foreach (var cortegeCar in cortegeCars)
            {
                var carToCortege = new CarOnCortege();
                carToCortege.powerLevel = (int)cortegeCar.Config.Level;

                var place = cortegeCar.GetComponent<PlaceComponent>().Place as CortegePlace;
                carToCortege.row = (int)place.Row;
                carToCortege.column = (int)place.Column;

                dataJSON.data.Add(carToCortege);
            }

            var stringDataJSON = JsonUtility.ToJson(dataJSON);
            _carToCortegeStorage.Save(stringDataJSON);
        }

        private void RestoreCarsOnParking()
        {
            try
            {
                ListDataJSON<CarOnParking> carsToParking = JsonUtility.FromJson<ListDataJSON<CarOnParking>>(_carToParkingStorage.Load());
                foreach (var carToPlace in carsToParking.data)
                {
                    var place = _parkingPlaces.Find(e => e.Number == carToPlace.placeNumber);
                    if (place == null) continue;

                    SpawnCar(new BuildParkingGuard((PowerLevel)carToPlace.powerLevel), place);
                }
            }
            catch
            {
                print("restore cars on parking failure");
            }
        }

        private void RestoreCarsOnCortege()
        {
            try
            {
                ListDataJSON<CarOnCortege> carsOnCortege = JsonUtility.FromJson<ListDataJSON<CarOnCortege>>(_carToCortegeStorage.Load());
                foreach (var carToPlace in carsOnCortege.data)
                {
                    var place = _cortegePlaces.Find(e => e.Row == (uint)carToPlace.row && e.Column == (uint)carToPlace.column);
                    if (place == null) continue;

                    SpawnCar(new BuildParkingGuard((PowerLevel)carToPlace.powerLevel), place);
                }
            }
            catch
            {
                print("restore cars on cortege failure");
            }
        }
        #endregion

        #region METHODS PUBLIC
        public List<AbstractCar> GetCortegeCars()
        {
            var parkingCars = FindObjectsOfType<AbstractCar>(true).ToList();
            var cortegeCars = parkingCars.FindAll(e => e.TryGetComponent<PlaceComponent>(out PlaceComponent component) && component.Place != null && component.Place is CortegePlace);

            return cortegeCars;
        }

        public int GetCortegeLevel()
        {
            var levelCount = 8;
            var levelSum = 0;

            var cortegeCars = GetCortegeCars();

            foreach (var car in cortegeCars)
            {
                if (car is GuardCar guard)
                {
                    levelSum += (int)guard.Config.Level;
                }
            }

            var cortegeLevel = levelSum / levelCount;
            cortegeLevel = cortegeLevel <= 0 ? 1 : cortegeLevel;

            return cortegeLevel;
        }
        #endregion
    }

    [Serializable]
    public struct ListDataJSON<T>
    {
        public List<T> data;
    }

    [Serializable]
    public struct CarOnParking
    {
        public int powerLevel;
        public int placeNumber;
    }

    [Serializable]
    public struct CarOnCortege
    {
        public int powerLevel;
        public int row;
        public int column;
    }
}
