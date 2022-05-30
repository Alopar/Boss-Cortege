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
        [SerializeField, Range(1, 100)] private float _speed = 0;
        #endregion

        #region FIELDS PRIVATE
        private bool _go = false;
        private Vector3 _startPosition;
        private List<CortegePoint> _points = new List<CortegePoint>();
        private List<RaidController> _raids = new List<RaidController>();
        private List<EnemyController> _enemies = new List<EnemyController>();

        private Coroutine _spawnEnemies;
        private Coroutine _spawnBulletEnemies;

        private static RaidController _limo;
        #endregion

        #region PROPERTIES
        public static RaidController Limo => _limo;
        #endregion


        #region HANDLERS
        private void SwipeDetection_OnSwipe(Vector2 direction)
        {
            if (!_go) return;

            ShiftCarRow(CortegeRow.One, (int)direction.x);
            ShiftCarRow(CortegeRow.Two, (int)direction.x);
            ShiftCarRow(CortegeRow.Three, (int)direction.x);
        }

        private void Raid_OnRaidDestroyed(RaidController raid)
        {
            DeleteCar(raid);

            if (raid.IsLimo)
            {
                GameManager.Instance.StopCortege();
            }
        }

        private void Enemy_OnEnemyDestroyed(EnemyController enemy)
        {
            DeleteEnemy(enemy);
        }
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _startPosition = transform.position;
            _points = FindObjectsOfType<CortegePoint>().ToList();
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

                    if (firstCar == null && secondCar != null && !secondCar.IsLimo)
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

                    if (firstCar == null && secondCar != null && !secondCar.IsLimo)
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

        private CortegePoint GetCortegePoint(CortegeRow row, CortegeColumn column)
        {
            return _points.Find(e => e.CortegeRow == row && e.CortegeColumn == column);
        }
        #endregion

        #region METHODS PUBLIC
        public void SetCar(CortegeRow row, CortegeColumn column, CarController car)
        {
            var raidCar = Instantiate(car.Settings.CarPrefab);
            raidCar.enabled = false;

            var raid = raidCar.gameObject.AddComponent<RaidController>();
            raid.Settings = car.Settings;
            raid.IsLimo = car.IsLimo;
            raid.Speed = _speed * 2f;

            raid.OnRaidDestroyed += Raid_OnRaidDestroyed;

            CortegePoint point = _points.Find(e => e.CortegeRow == row && e.CortegeColumn == column);
            if(point != null)
            {
                raid.transform.position = point.transform.position;
                raid.transform.parent = point.transform;
                raid.SetPoint(point);

                _raids.Add(raid);
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public void SetEnemy<T>(CortegeColumn column, T enemySchema) where T : Enemy
        {
            if(typeof(T) == typeof(SuicideEnemy))
            {
                var enemyCar = Instantiate(enemySchema.EnemyPrefab) as SuicideController;
                enemyCar.Settings = enemySchema as SuicideEnemy;

                CortegePoint startPoint = _points.Find(e => e.CortegeRow == CortegeRow.Front && e.CortegeColumn == column);
                CortegePoint finishPoint = _points.Find(e => e.CortegeRow == CortegeRow.Back && e.CortegeColumn == column);

                enemyCar.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;

                if (startPoint != null && finishPoint != null)
                {
                    enemyCar.transform.position = startPoint.transform.position;
                    enemyCar.transform.parent = startPoint.transform;
                    enemyCar.SetPoint(finishPoint);

                    _enemies.Add(enemyCar);
                }
                else
                {
                    throw new NullReferenceException();
                }
            }

            if (typeof(T) == typeof(BulletEnemy))
            {
                var enemyCar = Instantiate(enemySchema.EnemyPrefab) as BulletController;
                enemyCar.Settings = enemySchema as BulletEnemy;

                CortegePoint startPoint = _points.Find(e => e.CortegeRow == CortegeRow.Back && e.CortegeColumn == column);

                var randowRow = UnityEngine.Random.Range(1, 4);
                CortegePoint finishPoint = _points.Find(e => e.CortegeRow == (CortegeRow)randowRow && e.CortegeColumn == column);

                enemyCar.OnEnemyDestroyed += Enemy_OnEnemyDestroyed;

                if (startPoint != null && finishPoint != null)
                {
                    enemyCar.transform.position = startPoint.transform.position;
                    enemyCar.transform.parent = startPoint.transform;
                    enemyCar.SetPoint(finishPoint);

                    _enemies.Add(enemyCar);
                }
                else
                {
                    throw new NullReferenceException();
                }
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
            _limo = _raids.Find(e => e.IsLimo);
            _spawnEnemies = StartCoroutine(SpawnEnemies());
            _spawnBulletEnemies = StartCoroutine(SpawnBulletEnemies());
        }

        public void Stop()
        {
            _go = false;

            StopCoroutine(_spawnEnemies);
            StopCoroutine(_spawnBulletEnemies);

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

            var bullets = FindObjectsOfType<Bullet>();
            foreach (var bullet in bullets)
            {
                Destroy(bullet.gameObject);
            }
        }
        #endregion

        #region COROUTINES
        IEnumerator SpawnEnemies()
        {
            var timeDelay = 3f;
            while (true)
            {
                var enemySchema = Resources.Load<SuicideEnemy>("Suicide01");
                var randomColumn = UnityEngine.Random.Range(1, 4);

                SetEnemy<SuicideEnemy>((CortegeColumn)randomColumn, enemySchema);

                yield return new WaitForSeconds(timeDelay);
            }
        }

        IEnumerator SpawnBulletEnemies()
        {
            var timeDelay = 3f;
            while (true)
            {
                var enemySchema = Resources.Load<BulletEnemy>("Bullet01");
                var column = UnityEngine.Random.Range(0f, 1f) < 0.5f ? CortegeColumn.One : CortegeColumn.Five;
                var enemy = _enemies.Find(e => e.CortegePoint.CortegeColumn == column);
                if(enemy == null)
                {
                    SetEnemy<BulletEnemy>(column, enemySchema);
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
