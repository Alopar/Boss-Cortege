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
        private RaidManager _cortege;

        private uint _currentAvailableParkingPlaces;
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
                        var scheme = Resources.Load<GuardScheme>("Guard01");
                        SpawnCar(scheme, place);
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
            //var predicates = new List<Predicate<CortegePlace>>();
            //predicates.Add(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Two);
            //predicates.Add(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Three);
            //predicates.Add(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Four);
            //predicates.Add(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Two);
            //predicates.Add(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Three);
            //predicates.Add(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Four);
            //predicates.Add(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Two);
            //predicates.Add(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Three);
            //predicates.Add(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Four);

            //foreach (var p in predicates)
            //{
            //    var place = _cortegePlace.Find(p);
            //    if (!place.IsEmpty)
            //    {
            //        var car = place.Vechicle.GetCar();
            //        car.gameObject.SetActive(false);

            //        if (car.GetType() == typeof(GuardCar))
            //        {
            //            var scheme = (car as GuardCar).Config;
            //            _cortege.SetCar(place.CortegeRow, place.CortegeColumn, scheme);

            //            continue;
            //        }

            //        if (car.GetType() == typeof(BossCar))
            //        {
            //            var scheme = (car as BossCar).Config;
            //            _cortege.SetCar(place.CortegeRow, place.CortegeColumn, scheme);

            //            continue;
            //        }
            //    }
            //}

            //_cortege.Go();
            //_barrieController.UpBarrier();
        }

        private void StopRaceHandler(RaceStopInfo info)
        {
            //_cortege.Stop();
            //_barrieController.DownBarrier();

            //foreach (var place in _cortegePlace)
            //{
            //    var vechicle = place.Vechicle;
            //    if (vechicle == null) continue;

            //    var car = vechicle.GetCar();
            //    if (car == null) continue;

            //    car.gameObject.SetActive(true);
            //}
        }

        private void MergeCarHandler(MergeCarInfo info)
        {
            var dominantCar = info.FirstCar;
            var submissiveCar = info.SecondCar;

            string carSchemeName;
            switch (dominantCar.Config.Level)
            {
                case PowerLevel.Level01:
                    carSchemeName = "Guard02";
                    break;
                case PowerLevel.Level02:
                    carSchemeName = "Guard03";
                    break;
                case PowerLevel.Level03:
                    carSchemeName = "Guard04";
                    break;
                case PowerLevel.Level04:
                    carSchemeName = "Guard05";
                    break;
                case PowerLevel.Level05:
                    carSchemeName = "Guard06";
                    break;
                case PowerLevel.Level06:
                    carSchemeName = "Guard06";
                    break;
                default:
                    carSchemeName = "Guard06";
                    break;
            }
            var nextLevelCarScheme = Resources.Load<GuardScheme>(carSchemeName);

            dominantCar.Replace();
            Destroy(dominantCar.gameObject);

            SpawnCar(nextLevelCarScheme, submissiveCar.Replace());
            Destroy(submissiveCar.gameObject);
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
            _cortege = FindObjectOfType<RaidManager>();

            _cortegePlaces = FindObjectsOfType<CortegePlace>().ToList();
            _parkingPlaces = FindObjectsOfType<ParkingPlace>().OrderBy(e => e.Number).ToList();

            var bossPlace = _cortegePlaces.Find(e => e.IsBoss);
            var bossScheme = Resources.Load<BossScheme>("Limo01");
            SpawnCar(bossScheme, bossPlace);

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

        private void SpawnCar(BossScheme scheme, Place place)
        {
            var car = Instantiate(scheme.Prefab);
            car.Initialize(scheme, place);
            place.TryPlaceVechicle(car);
        }

        private void SpawnCar(GuardScheme scheme, Place place)
        {
            var car = Instantiate(scheme.Prefab);
            car.Initialize(scheme, place);

            car.gameObject.AddComponent<Merger>();

            place.TryPlaceVechicle(car);
        }
        #endregion
    }
}
