using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class RaceManager : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField, Range(1, 100), Tooltip("Монет за расстояние")] private uint _moneyPerUnit = 1;
        [SerializeField, Range(1, 100), Tooltip("Скорость всего кортежа")] private float _speed = 5;
        [SerializeField, Tooltip("Расстояние через которое усиливаются враги на один уровень")] private int _powerUpDistance = 200;

        [Space(10)]
        [Header("Spawner setting")]
        [SerializeField] private float _shootSpawnDelay = 3f;
        [SerializeField] private float _suicideSpawnDelay = 3f;
        [SerializeField] private float _barricadeSpawnDelay = 3f;
        #endregion

        #region FIELDS PRIVATE
        private bool _go = false;
        private Vector3 _startPosition;
        private List<RacePoint> _points;
        private List<AbstractEnemy> _enemies;

        private uint _raceMoney = 0;
        private uint _enemyMoney = 0;
        private uint _enemyDieCount = 0;
        private uint _distanceMoney = 0;

        private Coroutine _spawnShootEnemies;
        private Coroutine _spawnSuicideEnemies;
        private Coroutine _spawnBarricadeEnemies;

        private Cortege _cortege;
        private BossCar _boss;

        private static RaceManager _instance;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public BossCar Boss => _boss;
        public Cortege Cortege => _cortege;

        public uint RaceMoney => _raceMoney;
        public uint EnemyMoney => _enemyMoney;
        public uint EnemyCount => _enemyDieCount;
        public uint DistanceMoney => _distanceMoney;

        public static RaceManager Instance => _instance;
        #endregion

        #region HANDLERS
        private void RaceStartHandler(RaceStartInfo info)
        {
            Invoke(nameof(StartRace), 1.5f);
        }

        private void RaceStopHandler(RaceStopInfo info)
        {
            StopRace();
        }

        private void BossDieHandler(BossDieInfo info)
        {
            _distanceMoney = (uint)(Vector3.Distance(_startPosition, transform.position) * _moneyPerUnit);
            _raceMoney = _enemyMoney + _distanceMoney;
            EventHolder<RaceStopInfo>.NotifyListeners(null);
        }

        private void EnemyDieHandler(EnemyDieInfo info)
        {
            _enemyDieCount++;
            _enemyMoney += info.Money;
        }

        private void Enemy_OnEnemyDestroyed(AbstractEnemy enemy)
        {
            _enemies.Remove(enemy);
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<BossDieInfo>.AddListener(BossDieHandler, false);
            EventHolder<RaceStartInfo>.AddListener(RaceStartHandler, false);
            EventHolder<RaceStopInfo>.AddListener(RaceStopHandler, false);
            EventHolder<EnemyDieInfo>.AddListener(EnemyDieHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<BossDieInfo>.RemoveListener(BossDieHandler);
            EventHolder<RaceStartInfo>.RemoveListener(RaceStartHandler);
            EventHolder<RaceStopInfo>.RemoveListener(RaceStopHandler);
            EventHolder<EnemyDieInfo>.RemoveListener(EnemyDieHandler);
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

        private void FixedUpdate()
        {
            if (_go)
            {
                transform.position += Vector3.forward * _speed * Time.fixedDeltaTime;
                GameManager.Instance.Distance.SetDistance((uint)Vector3.Distance(_startPosition, transform.position));
            }
        }
        #endregion

        #region METHODS PRIVATE
        private void Init()
        {
            _startPosition = transform.position;
            _points = FindObjectsOfType<RacePoint>().ToList();
            _enemies = new List<AbstractEnemy>();
        }

        private CortegeCar SpawnCar(IBuildCarStrategy builder, CortegePlace place)
        {
            var car = builder.BuildCar();
            car.transform.position = place.StartRacePoint.transform.position;

            return new CortegeCar { Car = car, Row = place.Row, Column = place.Column, Spares = place.Spares, RacePoint = place.StartRacePoint };
        }

        private void StartRace()
        {
            _go = true;
            _raceMoney = 0;
            _enemyMoney = 0;
            _enemyDieCount = 0;
            _distanceMoney = 0;

            GameManager.Instance.Distance.SetDistance(0);

            _cortege = new Cortege();
            var carSpeed = _speed * 1f;
            var parkingCars = ParkingManager.Instance.GetCortegeCars();
            var cortegeCars = new List<CortegeCar>();
            foreach (var parkingCar in parkingCars)
            {
                var place = parkingCar.GetComponent<PlaceComponent>().Place as CortegePlace;
                if (place.IsBoss)
                {
                    var car = parkingCar as BossCar;
                    var cortegeCar = SpawnCar(new BuildRaceBoss(car.Config, carSpeed), place);
                    cortegeCars.Add(cortegeCar);

                    _boss = cortegeCar.Car as BossCar;
                }
                else
                {
                    var car = parkingCar as GuardCar;
                    var cortegeCar = SpawnCar(new BuildRaceGuard(car.Config, carSpeed), place);
                    cortegeCars.Add(cortegeCar);
                }
            }
            _cortege.Init(cortegeCars);

            _spawnShootEnemies = StartCoroutine(SpawnShootEnemies(_shootSpawnDelay));
            _spawnSuicideEnemies = StartCoroutine(SpawnSuicideEnemies(_suicideSpawnDelay));
            _spawnBarricadeEnemies = StartCoroutine(SpawnBarricadeEnemies(_barricadeSpawnDelay));
        }

        private void StopRace()
        {
            _go = false;

            _cortege.Dispose();

            StopCoroutine(_spawnShootEnemies);
            StopCoroutine(_spawnSuicideEnemies);
            StopCoroutine(_spawnBarricadeEnemies);

            _enemies.ForEach(e => Destroy(e.gameObject));
            _enemies.Clear();

            transform.position = _startPosition;
        }
        #endregion

        #region METHODS PUBLIC
        private AbstractEnemy SpawnEnemy(IBuildEnemyStrategy builder)
        {
            var enemy = builder.BuildEnemy();
            enemy.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;

            _enemies.Add(enemy);

            return enemy;
        }

        public RacePoint GetRacePoint(uint row, uint column)
        {
            return _points.Find(e => e.Row == row && e.Column == column);
        }
        #endregion

        #region COROUTINES
        IEnumerator SpawnShootEnemies(float delay)
        {
            var cortegeLevel = ParkingManager.Instance.GetCortegeLevel();

            while (true)
            {
                yield return new WaitForSeconds(delay);

                var distance = Vector3.Distance(_startPosition, transform.position);
                var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
                var enemySchemeName = $"Shoot0{Mathf.Clamp(cortegeLevel + enemyPowerUp, 1, 6)}";

                var enemySchema = Resources.Load<ShootEnemyScheme>(enemySchemeName);
                var randomColumn = UnityEngine.Random.Range(0f, 1f) < 0.5f ? 0 : 4;

                var occupiedEnemy = _enemies.Find(e => e.TryGetComponent<MoveComponent>(out var component) && component.Point.Column == randomColumn);
                if (occupiedEnemy != null) continue;

                var occupiedPoint = _cortege.GetOccupiedPoints().Find(e => e.Column == randomColumn);
                if (occupiedPoint != null) continue;

                var startPoint = GetRacePoint(0, (uint)randomColumn);
                var finishPoint = GetRacePoint(3, (uint)randomColumn);

                var enemy = SpawnEnemy(new BuildShootEnemy(enemySchema, _speed));

                enemy.transform.position = startPoint.transform.position;
                enemy.GetComponent<MoveComponent>().SetPoint(finishPoint);
            }
        }

        IEnumerator SpawnSuicideEnemies(float delay)
        {
            var cortegeLevel = ParkingManager.Instance.GetCortegeLevel();

            while (true)
            {
                yield return new WaitForSeconds(delay);

                var distance = Vector3.Distance(_startPosition, transform.position);
                var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
                var enemySchemeName = $"Suicide0{Mathf.Clamp(cortegeLevel + enemyPowerUp, 1, 6)}";

                var enemySchema = Resources.Load<SuicideEnemyScheme>(enemySchemeName);
                var randomColumn = UnityEngine.Random.Range(1, 4);
                var startPoint = GetRacePoint(4, (uint)randomColumn);
                var finishPoint = GetRacePoint(0, (uint)randomColumn);

                var enemy = SpawnEnemy(new BuildSuicideEnemy(enemySchema));

                enemy.transform.position = startPoint.transform.position;
                enemy.GetComponent<MoveComponent>().SetPoint(finishPoint);
            }
        }

        IEnumerator SpawnBarricadeEnemies(float delay)
        {
            var cortegeLevel = ParkingManager.Instance.GetCortegeLevel();

            while (true)
            {
                yield return new WaitForSeconds(delay);

                var distance = Vector3.Distance(_startPosition, transform.position);
                var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
                var enemySchemeName = $"Barricade0{Mathf.Clamp(cortegeLevel + enemyPowerUp, 1, 6)}";

                var enemySchema = Resources.Load<BarricadeEnemyScheme>(enemySchemeName);
                var randomColumn = UnityEngine.Random.Range(1, 4);
                var placePoint = GetRacePoint(4, (uint)randomColumn);

                var enemy = SpawnEnemy(new BuildBarricadeEnemy(enemySchema));

                enemy.transform.position = placePoint.transform.position;
            }
        }
        #endregion
    }
}
