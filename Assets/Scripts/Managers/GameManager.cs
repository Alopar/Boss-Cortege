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

        [Space(10)]
        [SerializeField] private Parking _parking;
        [SerializeField] private BarrieController _barrieController;
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
        public event Action<int> OnDistanceChange;
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
            var limoScheme = Resources.Load<LimoScheme>("Limo01");
            SpawnCar(limoScheme, limoPlace);

            SetMoney(1000);
        }
        #endregion

        #region METHODS PRIVATE
        private void SpawnCar(LimoScheme scheme, Place place)
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
        public void SetDistance(int value)
        {
            OnDistanceChange?.Invoke(value);
        }

        public void SetMoney(uint coins)
        {
            _money += (int)coins;
            OnMoneyChange?.Invoke(_money);
        }

        public void BuyPlace(uint cost)
        {
            if (GetMoney(cost))
            {
                _parking.AddPlace();
            }
        }

        public void BuyCar(uint cost)
        {
            foreach (var place in _parkingPlace)
            {
                if(place.IsVacant && place.IsEmpty)
                {   
                    if (GetMoney(cost))
                    {
                        var scheme = Resources.Load<GuardScheme>("Guard01");
                        SpawnCar(scheme, place);
                    }

                    return;
                }
            }
        }

        public void MergeCar(Merger dominantCar, Merger submissiveCar)
        {
            string carSchemeName;
            switch (dominantCar.Parking.Config.Level)
            {
                case CarLevel.Level01:
                    carSchemeName = "Guard02";
                    break;
                case CarLevel.Level02:
                    carSchemeName = "Guard03";
                    break;
                case CarLevel.Level03:
                    carSchemeName = "Guard04";
                    break;
                case CarLevel.Level04:
                    carSchemeName = "Guard05";
                    break;
                case CarLevel.Level05:
                    carSchemeName = "Guard06";
                    break;
                case CarLevel.Level06:
                    carSchemeName = "Guard06";
                    break;
                default:
                    carSchemeName = "Guard06";
                    break;
            }
            var nextLevelCarScheme = Resources.Load<GuardScheme>(carSchemeName);

            dominantCar.Parking.Replace();
            Destroy(dominantCar.gameObject);
            
            SpawnCar(nextLevelCarScheme, submissiveCar.Parking.Replace());
            Destroy(submissiveCar.gameObject);
        }

        public void SwapCar(Merger dominantCar, Merger submissiveCar)
        {
            var dominantCarPlace = dominantCar.Parking.Replace();
            var submissiveCarPlace = submissiveCar.Parking.Replace();

            dominantCarPlace.TryPlaceVechicle(submissiveCar.Parking);
            submissiveCarPlace.TryPlaceVechicle(dominantCar.Parking);
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
                if (!place.IsEmpty)
                {
                    var car = place.Vechicle.GetCar();
                    car.gameObject.SetActive(false);

                    if(car.GetType() == typeof(GuardParkingController))
                    {
                        var scheme = (car as GuardParkingController).Config;
                        _cortege.SetCar(place.CortegeRow, place.CortegeColumn, scheme);

                        continue;
                    }

                    if (car.GetType() == typeof(LimoParkingController))
                    {
                        var scheme = (car as LimoParkingController).Config;
                        _cortege.SetCar(place.CortegeRow, place.CortegeColumn, scheme);

                        continue;
                    }
                }
            }

            _cortege.Go();
            _cortegeCamera.Priority = 30;
            _barrieController.UpBarrier();
        }

        public void StopCortege()
        {
            _cortege.Stop();
            _cortegeCamera.Priority = 10;
            _barrieController.DownBarrier();

            foreach (var place in _cortegePlace)
            {
                var vechicle = place.Vechicle;
                if (vechicle == null) continue;

                var car = vechicle.GetCar();
                if (car == null) continue;

                car.gameObject.SetActive(true);
            }

            OnCortegeStop?.Invoke();
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