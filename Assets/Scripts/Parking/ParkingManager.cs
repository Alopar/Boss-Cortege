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

        [Space(10)]
        [SerializeField] private GameObject _spawnCarVFX;
        [SerializeField] private GameObject _mergeCarVFX;
        #endregion

        #region FIELDS PRIVATE
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
                UnlockPlaces();
            }
        }

        private void RaceStartHandler(RaceStartInfo info)
        {
            Invoke(nameof(UpBarrier), 1.5f);
            Invoke(nameof(HideCortegeCars), 1.5f);
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

            var nextLevel = (PowerLevel)Mathf.Clamp((int)dominantCar.Config.Level + 1, 1, 6);
            var place = submissivePlace.Replace();

            dominantPlace.Replace();
            Destroy(dominantCar.gameObject);
            Destroy(submissiveCar.gameObject);

            SpawnCar(new BuildParkingGuard(nextLevel), place);
            Instantiate(_mergeCarVFX, place.SpawnPoint.position, Quaternion.identity);
        }

        private void SwapCarHandler(SwapCarInfo info)
        {
            var dominantPlaceComponent = info.FirstPlace;
            var submissivePlaceComponent = info.SecondPlace;

            var dominantCarPlace = dominantPlaceComponent.Replace();
            var submissiveCarPlace = submissivePlaceComponent.Replace();

            dominantCarPlace.PlaceVechicle(submissivePlaceComponent);
            submissiveCarPlace.PlaceVechicle(dominantPlaceComponent);
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
        }

        private void OnDisable()
        {
            EventHolder<BuyCarInfo>.RemoveListener(BuyCarHandler);
            EventHolder<BuyPlaceInfo>.RemoveListener(BuyPlaceHandler);
            EventHolder<RaceStartInfo>.RemoveListener(RaceStartHandler);
            EventHolder<RaceStopInfo>.RemoveListener(RaceStopHandler);
            EventHolder<MergeCarInfo>.RemoveListener(MergeCarHandler);
            EventHolder<SwapCarInfo>.RemoveListener(SwapCarHandler);
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
            _currentAvailableParkingPlaces = _baseAvailableParkingPlaces;

            _cortegePlaces = FindObjectsOfType<CortegePlace>().ToList();
            _parkingPlaces = FindObjectsOfType<ParkingPlace>().OrderBy(e => e.Number).ToList();

            var bossPlace = _cortegePlaces.Find(e => e.IsBoss);
            SpawnCar(new BuildParkingBoss(), bossPlace);
            UnlockPlaces();
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
}
