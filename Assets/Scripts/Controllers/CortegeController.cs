using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class CortegeController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField, Range(1, 100), Tooltip("����� �� ����������")] private uint _moneyPerUnit = 1;
        [SerializeField, Range(1, 100), Tooltip("�������� ����� �������")] private float _speed = 0;

        [Space(10)]
        [SerializeField] private Transform _raidsContainer;
        [SerializeField] private Transform _enemiesContainer;
        [SerializeField] private Transform _projectilesContainer;
        #endregion

        #region FIELDS PRIVATE
        private bool _go = false;
        private Vector3 _startPosition;
        private List<CortegePoint> _points = new List<CortegePoint>();
        private List<RaidController> _raids = new List<RaidController>();
        private List<EnemyController> _enemies = new List<EnemyController>();

        private Coroutine _spawnEnemies;
        private Coroutine _spawnBulletEnemies;

        private RaidController _limo;

        private static CortegeController _instance;
        #endregion

        #region PROPERTIES
        public RaidController Limo => _limo;

        public Transform RaidsContainer => _raidsContainer;
        public Transform EnemiesContainer => _enemiesContainer;
        public Transform ProjectilesContainer => _projectilesContainer;

        public static CortegeController Instance => _instance;
        #endregion

        #region HANDLERS
        private void SwipeDetection_OnSwipe(Vector2 direction)
        {
            if (!_go) return;

            ShiftCarRow(CortegeRow.One, (int)direction.x);
            ShiftCarRow(CortegeRow.Two, (int)direction.x);
            ShiftCarRow(CortegeRow.Three, (int)direction.x);
        }

        private void Limo_OnLimoDestroyed()
        {
            GameManager.Instance.StopCortege();
        }

        private void Raid_OnRaidDestroyed(RaidController raid)
        {
            DeleteCar(raid);
        }

        private void Enemy_OnEnemyDestroyed(EnemyController enemy)
        {
            DeleteEnemy(enemy);
        }
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                _startPosition = transform.position;
                _points = FindObjectsOfType<CortegePoint>().ToList();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (_go)
            {
                transform.position += Vector3.forward * _speed * Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            SwipeDetection.OnSwipe += SwipeDetection_OnSwipe;
        }

        private void OnDisable()
        {
            SwipeDetection.OnSwipe -= SwipeDetection_OnSwipe;
        }
        #endregion

        #region METHODS PRIVATE
        private void ShiftCarRow(CortegeRow row, int direction)
        {
            if(direction < 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    var firstCar = GetCortegeCar(row, (CortegeColumn)i);
                    var secondCar = GetCortegeCar(row, (CortegeColumn)(i + 1));

                    if (firstCar == null && secondCar != null)
                    {
                        secondCar.SetPoint(GetCortegePoint(row, (CortegeColumn)i));
                    }
                }
            }
            else
            {
                for (int i = 4; i > 0; i--)
                {
                    var firstCar = GetCortegeCar(row, (CortegeColumn)i);
                    var secondCar = GetCortegeCar(row, (CortegeColumn)(i - 1));

                    if (firstCar == null && secondCar != null)
                    {
                        secondCar.SetPoint(GetCortegePoint(row, (CortegeColumn)i));
                    }
                }
            }
        }

        private RaidController GetCortegeCar(CortegeRow row, CortegeColumn column)
        {
            return _raids.Find(e => e.CortegePoint.CortegeRow == row && e.CortegePoint.CortegeColumn == column);
        }
        #endregion

        #region METHODS PUBLIC
        public CortegePoint GetCortegePoint(CortegeRow row, CortegeColumn column)
        {
            return _points.Find(e => e.CortegeRow == row && e.CortegeColumn == column);
        }

        public void SetCar<T>(CortegeRow row, CortegeColumn column, T carScheme) where T : CarScheme
        {
            if (typeof(T) == typeof(GuardScheme))
            {
                var raidSchema = carScheme as GuardScheme;
                var raidCar = Instantiate(raidSchema.Prefab);
                raidCar.transform.SetParent(_raidsContainer);
                raidCar.enabled = false;

                var raidController = raidCar.gameObject.AddComponent<GuardRaidController>();
                raidController.Initialize(raidSchema);
                raidController.Speed = _speed * 2f;

                raidController.OnRaidDestroyed += Raid_OnRaidDestroyed;

                CortegePoint point = _points.Find(e => e.CortegeRow == row && e.CortegeColumn == column);
                if (point != null)
                {
                    raidController.transform.position = point.transform.position;
                    raidController.SetPoint(point);

                    _raids.Add(raidController);
                }
                else
                {
                    throw new NullReferenceException();
                }

                return;
            }

            if (typeof(T) == typeof(LimoScheme))
            {
                var raidSchema = carScheme as LimoScheme;
                var raidCar = Instantiate(raidSchema.Prefab);
                raidCar.transform.SetParent(_raidsContainer);
                raidCar.enabled = false;

                var raidController = raidCar.gameObject.AddComponent<LimoRaidController>();
                raidController.Initialize(raidSchema);
                raidController.Speed = _speed * 2f;

                raidController.OnRaidDestroyed += Raid_OnRaidDestroyed;
                raidController.OnLimoDestroyed += Limo_OnLimoDestroyed;

                CortegePoint point = _points.Find(e => e.CortegeRow == row && e.CortegeColumn == column);
                if (point != null)
                {
                    raidController.transform.position = point.transform.position;
                    raidController.SetPoint(point);

                    _raids.Add(raidController);
                }
                else
                {
                    throw new NullReferenceException();
                }

                return;
            }
        }

        public void SetEnemy<T>(CortegeColumn column, T enemySchema) where T : EnemyScheme
        {
            if(typeof(T) == typeof(SuicideEnemyScheme))
            {
                var enemyCar = Instantiate(enemySchema.Prefab) as SuicideEnemyController;
                enemyCar.transform.SetParent(_enemiesContainer);
                enemyCar.Config = enemySchema as SuicideEnemyScheme;

                CortegePoint startPoint = _points.Find(e => e.CortegeRow == CortegeRow.Front && e.CortegeColumn == column);
                CortegePoint finishPoint = _points.Find(e => e.CortegeRow == CortegeRow.Back && e.CortegeColumn == column);

                enemyCar.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;

                if (startPoint != null && finishPoint != null)
                {
                    enemyCar.transform.position = startPoint.transform.position;                    
                    enemyCar.SetPoint(finishPoint);

                    _enemies.Add(enemyCar);
                }
                else
                {
                    throw new NullReferenceException();
                }

                return;
            }

            if (typeof(T) == typeof(ShootEnemyScheme))
            {
                var enemyCar = Instantiate(enemySchema.Prefab) as ShootEnemyController;
                enemyCar.transform.SetParent(_enemiesContainer);
                enemyCar.Config = enemySchema as ShootEnemyScheme;

                CortegePoint startPoint = _points.Find(e => e.CortegeRow == CortegeRow.Back && e.CortegeColumn == column);

                var randowRow = UnityEngine.Random.Range(1, 4);
                CortegePoint finishPoint = _points.Find(e => e.CortegeRow == (CortegeRow)randowRow && e.CortegeColumn == column);

                enemyCar.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;

                if (startPoint != null && finishPoint != null)
                {
                    enemyCar.transform.position = startPoint.transform.position;
                    enemyCar.SetPoint(finishPoint);

                    _enemies.Add(enemyCar);
                }
                else
                {
                    throw new NullReferenceException();
                }

                return;
            }

        }

        public void DeleteCar(RaidController raid)
        {
            _raids.Remove(raid);
            raid.OnRaidDestroyed -= Raid_OnRaidDestroyed;
        }

        public void DeleteEnemy(EnemyController enemy)
        {
            _enemies.Remove(enemy);
            enemy.OnEnemyDestroyed -= Enemy_OnEnemyDestroyed;
        }

        public void Go()
        {
            _go = true;
            _limo = _raids.Find(e => e.GetType() == typeof(LimoRaidController));
            _spawnEnemies = StartCoroutine(SpawnSuicideEnemies());
            _spawnBulletEnemies = StartCoroutine(SpawnShootEnemies());
        }

        public void Stop()
        {
            _go = false;

            StopCoroutine(_spawnEnemies);
            StopCoroutine(_spawnBulletEnemies);

            var coins = Vector3.Distance(_startPosition, transform.position) * _moneyPerUnit;
            GameManager.Instance.SetMoney((uint)coins);

            transform.position = _startPosition;

            var raidsToDelete = _raids.ToArray();
            foreach (var raid in raidsToDelete)
            {
                raid.Die();
            }

            var enemiesToDelete = _enemies.ToArray();
            foreach (var enemy in enemiesToDelete)
            {
                enemy.Die();
            }

            var bullets = FindObjectsOfType<ProjectileController>();
            foreach (var bullet in bullets)
            {
                Destroy(bullet.gameObject);
            }
        }
        #endregion

        #region COROUTINES
        IEnumerator SpawnSuicideEnemies()
        {
            var timeDelay = 3f;
            while (true)
            {
                var enemySchema = Resources.Load<SuicideEnemyScheme>("Suicide01");
                var randomColumn = UnityEngine.Random.Range(1, 4);

                SetEnemy<SuicideEnemyScheme>((CortegeColumn)randomColumn, enemySchema);

                yield return new WaitForSeconds(timeDelay);
            }
        }

        IEnumerator SpawnShootEnemies()
        {
            var timeDelay = 3f;
            while (true)
            {
                var enemySchema = Resources.Load<ShootEnemyScheme>("Shoot01");
                var column = UnityEngine.Random.Range(0f, 1f) < 0.5f ? CortegeColumn.One : CortegeColumn.Five;
                
                var enemy = _enemies.Find(e => e.CortegePoint.CortegeColumn == column);
                var raid = _raids.Find(e => e.CortegePoint.CortegeColumn == column);
                if(enemy == null && raid == null)
                {
                    SetEnemy(column, enemySchema);
                }

                yield return new WaitForSeconds(timeDelay);
            }
        }
        #endregion
    }

    public enum CortegeRow
    {
        Back = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Front = 4
    }

    public enum CortegeColumn
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4
    }
}
