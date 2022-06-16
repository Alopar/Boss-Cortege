using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BossCortege.EventHolder;
using Random = UnityEngine.Random;

namespace BossCortege
{
    public class ParkingManager : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private uint _baseAvailableParkingPlaces = 3;

        [Space(10)]
        [SerializeField] private BarrieController _barrieController;
        #endregion

        #region FIELDS PRIVATE
        private List<CortegePlace> _cortegePlaces;
        private List<ParkingPlace> _parkingPlaces;
        private RaceManager _raceManager;

        private uint _currentAvailableParkingPlaces;

        private ICarFactory _carFactory = new CarFactory();
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
                        SpawnCar(new GuardCarFactoryStrategy(PowerLevel.Level01), place);
                    }

                    return;
                }
            }
        }

        private void BuyPlaceHandler(BuyPlaceInfo info)
        {
            _currentAvailableParkingPlaces++;
            UnlockPlaces();
        }

        private void StartRaceHandler(RaceStartInfo info)
        {
            var cortegeCars = GetCortegeCars();
            foreach (var car in cortegeCars)
            {
                car.gameObject.SetActive(false);
                //_raceManager.SetCar();
            }

            _raceManager.Go();
            _barrieController.Invoke(nameof(_barrieController.UpBarrier), 0.5f);
        }

        private void StopRaceHandler(RaceStopInfo info)
        {
            _raceManager.Stop();
            _barrieController.DownBarrier();

            var cortegeCars = GetCortegeCars();
            cortegeCars.ForEach(e => e.gameObject.SetActive(true));
        }

        private void MergeCarHandler(MergeCarInfo info)
        {
            var dominantCar = info.FirstCar;
            var submissiveCar = info.SecondCar;

            var nextLevel = (PowerLevel)Mathf.Clamp((int)dominantCar.Config.Level + 1, 1, 6);
            var place = submissiveCar.Replace();

            dominantCar.Replace();
            Destroy(dominantCar.gameObject);
            Destroy(submissiveCar.gameObject);

            SpawnCar(new GuardCarFactoryStrategy(nextLevel), place);
        }

        private void SwapCarHandler(SwapCarInfo info)
        {
            var dominantCar = info.FirstCar;
            var submissiveCar = info.SecondCar;

            var dominantCarPlace = dominantCar.Replace();
            var submissiveCarPlace = submissiveCar.Replace();

            dominantCarPlace.TryPlaceVechicle(submissiveCar);
            submissiveCarPlace.TryPlaceVechicle(dominantCar);
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<BuyCarInfo>.AddListener(BuyCarHandler, false);
            EventHolder<BuyPlaceInfo>.AddListener(BuyPlaceHandler, false);
            EventHolder<RaceStartInfo>.AddListener(StartRaceHandler, false);
            EventHolder<RaceStopInfo>.AddListener(StopRaceHandler, false);
            EventHolder<MergeCarInfo>.AddListener(MergeCarHandler, false);
            EventHolder<SwapCarInfo>.AddListener(SwapCarHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<BuyCarInfo>.RemoveListener(BuyCarHandler);
            EventHolder<BuyPlaceInfo>.RemoveListener(BuyPlaceHandler);
            EventHolder<RaceStartInfo>.RemoveListener(StartRaceHandler);
            EventHolder<RaceStopInfo>.RemoveListener(StopRaceHandler);
            EventHolder<MergeCarInfo>.RemoveListener(MergeCarHandler);
            EventHolder<SwapCarInfo>.RemoveListener(SwapCarHandler);
        }

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
        private void Init()
        {
            _raceManager = FindObjectOfType<RaceManager>();

            _cortegePlaces = FindObjectsOfType<CortegePlace>().ToList();
            _parkingPlaces = FindObjectsOfType<ParkingPlace>().OrderBy(e => e.Number).ToList();

            var bossPlace = _cortegePlaces.Find(e => e.IsBoss);
            SpawnCar(new BossCarFactoryStrategy(), bossPlace);
            UnlockPlaces();
        }

        private List<AbstractCar> GetCortegeCars()
        {
            var parkingCars = FindObjectsOfType<AbstractCar>(true).ToList();
            var cortegeCars = parkingCars.FindAll(e => e.Place != null && e.Place is CortegePlace);

            return cortegeCars;
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

        private void SpawnCar(ICarFactoryStrategy strategy, AbstractPlace place)
        {
            var car = _carFactory.CreateCar(strategy);
            if (place.TryPlaceVechicle(car))
            {
                car.SetPlace(place);
            }
            else
            {
                print("Place not available");
                Destroy(car.gameObject);
            }
        }
        #endregion
    }
}
