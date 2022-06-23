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

        public static RaceManager Instance => _instance;
        #endregion

        #region HANDLERS
        private void RaceStartHandler(RaceStartInfo info)
        {
            StartRace();
        }

        private void RaceStopHandler(RaceStopInfo info)
        {
            StopRace();
        }

        private void BossDieHandler(BossDieInfo info)
        {
            EventHolder<RaceStopInfo>.NotifyListeners(null);
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
        }

        private void OnDisable()
        {
            EventHolder<BossDieInfo>.RemoveListener(BossDieHandler);
            EventHolder<RaceStartInfo>.RemoveListener(RaceStartHandler);
            EventHolder<RaceStopInfo>.RemoveListener(RaceStopHandler);
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
            }

            GameManager.Instance.Distance.SetDistance((uint)Vector3.Distance(_startPosition, transform.position));
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

            //_spawnShootEnemies = StartCoroutine(SpawnShootEnemies());
            //_spawnSuicideEnemies = StartCoroutine(SpawnSuicideEnemies());
            _spawnBarricadeEnemies = StartCoroutine(SpawnBarricadeEnemies(_barricadeSpawnDelay));
        }

        private void StopRace()
        {
            _go = false;

            _cortege.Dispose();

            //StopCoroutine(_spawnShootEnemies);
            //StopCoroutine(_spawnSuicideEnemies);
            StopCoroutine(_spawnBarricadeEnemies);

            var coins = Vector3.Distance(_startPosition, transform.position) * _moneyPerUnit;
            GameManager.Instance.Wallet.SetCash((uint)coins);

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

        //public void SetEnemy<T>(CortegeColumn column, T enemySchema) where T : EnemyScheme
        //{
        //    if (typeof(T) == typeof(ShootEnemyScheme))
        //    {
        //        var startPoint = _points.Find(e => e.CortegeRow == CortegeRow.Back && e.CortegeColumn == column);
        //        enemy.transform.position = startPoint.transform.position;

        //        var randowRow = UnityEngine.Random.Range(1, 4);
        //        var finishPoint = _points.Find(e => e.CortegeRow == (CortegeRow)randowRow && e.CortegeColumn == column);
        //        enemy.SetPoint(finishPoint);
        //    }

        //    if (typeof(T) == typeof(SuicideEnemyScheme))
        //    {
        //        var startPoint = _points.Find(e => e.CortegeRow == CortegeRow.Front && e.CortegeColumn == column);
        //        enemy.transform.position = startPoint.transform.position;

        //        var finishPoint = _points.Find(e => e.CortegeRow == CortegeRow.Back && e.CortegeColumn == column);
        //        enemy.SetPoint(finishPoint);
        //    }
        //}

        public RacePoint GetRacePoint(uint row, uint column)
        {
            return _points.Find(e => e.Row == row && e.Column == column);
        }
        #endregion

        #region COROUTINES
        //IEnumerator SpawnShootEnemies()
        //{
        //    var cortegeLevel = GetCortegeLevel();

        //    var timeDelay = 5f;
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(timeDelay);

        //        var distance = Vector3.Distance(_startPosition, transform.position);
        //        var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
        //        var enemyName = $"Shoot0{cortegeLevel + enemyPowerUp}";

        //        var enemySchema = Resources.Load<ShootEnemyScheme>(enemyName);
        //        var column = UnityEngine.Random.Range(0f, 1f) < 0.5f ? CortegeColumn.One : CortegeColumn.Five;

        //        var enemy = _enemies.Find(e => e.CortegePoint.CortegeColumn == column);
        //        var raid = _raids.Find(e => e.CortegePoint.CortegeColumn == column);
        //        if (enemy == null && raid == null)
        //        {
        //            SetEnemy(column, enemySchema);
        //        }
        //    }
        //}

        //IEnumerator SpawnSuicideEnemies()
        //{
        //    var cortegeLevel = GetCortegeLevel();

        //    var timeDelay = 3f;
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(timeDelay);

        //        var distance = Vector3.Distance(_startPosition, transform.position);
        //        var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
        //        var enemyName = $"Suicide0{cortegeLevel + enemyPowerUp}";

        //        var enemySchema = Resources.Load<SuicideEnemyScheme>(enemyName);
        //        var randomColumn = UnityEngine.Random.Range(1, 4);

        //        SetEnemy((CortegeColumn)randomColumn, enemySchema);
        //    }
        //}

        IEnumerator SpawnBarricadeEnemies(float delay)
        {
            var cortegeLevel = ParkingManager.Instance.GetCortegeLevel();

            while (true)
            {
                yield return new WaitForSeconds(delay);

                var distance = Vector3.Distance(_startPosition, transform.position);
                var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
                var enemySchemeName = $"Barricade0{cortegeLevel + enemyPowerUp}";

                var enemySchema = Resources.Load<BarricadeEnemyScheme>(enemySchemeName);
                var randomColumn = UnityEngine.Random.Range(1, 3);
                var point = _points.Find(e => e.Column == randomColumn && e.Row == 4);

                var enemy = SpawnEnemy(new BuildBarricadeEnemy(enemySchema));

                enemy.transform.position = point.transform.position;
            }
        }
        #endregion
    }
}
