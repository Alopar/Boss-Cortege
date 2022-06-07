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
        [SerializeField, Range(1, 100), Tooltip("Монет за расстояние")] private uint _moneyPerUnit = 1;
        [SerializeField, Range(1, 100), Tooltip("Скорость всего кортежа")] private float _speed = 5;
        [SerializeField, Range(1, 100), Tooltip("Скорость перестроения машин")] private float _shiftSpeed = 3;
        [SerializeField, Tooltip("Расстояние через которое усиливаются враги на один уровень")] private int _powerUpDistance = 200;

        [Space(10)]
        [SerializeField] private Transform _raidsContainer;
        [SerializeField] private Transform _enemiesContainer;
        [SerializeField] private Transform _projectilesContainer;
        #endregion

        #region FIELDS PRIVATE
        private bool _isAttackLeft = false;
        private bool _isAttackRight = false;

        private bool _go = false;
        private Vector3 _startPosition;
        private List<CortegePoint> _points = new List<CortegePoint>();
        private List<RaidController> _raids = new List<RaidController>();
        private List<EnemyController> _enemies = new List<EnemyController>();

        private Coroutine _spawnEnemies;
        private Coroutine _spawnBulletEnemies;

        private Cortege _cortege;
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
            if (_isAttackLeft || _isAttackRight) return;

            if(direction.x < 0)
            {
                var checkCell = _cortege.GetCellByPosition(CortegePosition.Front, CortegePosition.Left);
                if (checkCell.Point.LeftPoint == null) return;

                var enemy = GetEnemyByColumn(checkCell.Point.LeftPoint.CortegeColumn);
                if (enemy != null && enemy.GetType() == typeof(ShootEnemyController))
                {
                    _isAttackLeft = true;
                    if (enemy.CortegePoint.RightPoint.RaidController != null)
                    {
                        
                        _cortege.MoveLeft();
                    }
                    else
                    {
                        StartCoroutine(FalseAttack(true));
                    }
                }
                else
                {
                    _cortege.MoveLeft();
                }
            }
            else
            {
                var checkCell = _cortege.GetCellByPosition(CortegePosition.Front, CortegePosition.Right);
                if (checkCell.Point.RightPoint == null) return;

                var enemy = GetEnemyByColumn(checkCell.Point.RightPoint.CortegeColumn);
                if (enemy != null && enemy.GetType() == typeof(ShootEnemyController))
                {
                    _isAttackRight = true;
                    if (enemy.CortegePoint.LeftPoint.RaidController != null)
                    {   
                        _cortege.MoveRight();
                    }
                    else
                    {
                        StartCoroutine(FalseAttack(false));
                    }
                }
                else
                {
                    _cortege.MoveRight();
                }
            }

            //ShiftCarRow(CortegeRow.One, (int)direction.x);
            //ShiftCarRow(CortegeRow.Two, (int)direction.x);
            //ShiftCarRow(CortegeRow.Three, (int)direction.x);
        }

        private void Limo_OnLimoDestroyed()
        {
            GameManager.Instance.StopCortege();
        }

        private void Raid_OnRam()
        {
            if (_isAttackLeft)
            {
                _cortege.MoveRight();
            }

            if (_isAttackRight)
            {   
                _cortege.MoveLeft();
            }
        }

        private void Raid_OnRaidDestroyed(RaidController raid)
        {
            var cell = _cortege.GetCellByRaid(raid);
            if(cell != null)
            {
                if(cell.Vertical == CortegePosition.Front && cell.Horizontal != CortegePosition.Center)
                {
                    var vacantCell = _cortege.GetCellByPosition(CortegePosition.Front, CortegePosition.Center);
                    if(vacantCell.Raid != null)
                    {
                        cell.Raid = vacantCell.Raid;
                        cell.Raid.SetPoint(cell.Point);
                        vacantCell.Raid = null;
                    }
                }
                
                if (cell.Vertical == CortegePosition.Middle && cell.Horizontal != CortegePosition.Center)
                {
                    var centerVacantCell = _cortege.GetCellByPosition(CortegePosition.Middle, CortegePosition.Center);
                    var bottomVacantCell = _cortege.GetCellByPosition(CortegePosition.Back, cell.Horizontal);
                    if (centerVacantCell.Raid != null)
                    {
                        cell.Raid = centerVacantCell.Raid;
                        cell.Raid.SetPoint(cell.Point);
                        centerVacantCell.Raid = null;
                    }
                    else if(bottomVacantCell.Raid != null)
                    {
                        cell.Raid = bottomVacantCell.Raid;
                        cell.Raid.SetPoint(cell.Point);
                        bottomVacantCell.Raid = null;
                    }
                }
            }

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

        private void FixedUpdate()
        {
            if (_go)
            {
                transform.position += Vector3.forward * _speed * Time.fixedDeltaTime;
            }

            GameManager.Instance.SetDistance((int)Vector3.Distance(_startPosition, transform.position));
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
        //private void ShiftCarRow(CortegeRow row, int direction)
        //{
        //    if(direction < 0)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            var firstCar = GetCortegeCar(row, (CortegeColumn)i);
        //            var secondCar = GetCortegeCar(row, (CortegeColumn)(i + 1));

        //            if (firstCar == null && secondCar != null)
        //            {
        //                secondCar.SetPoint(GetCortegePoint(row, (CortegeColumn)i));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 4; i > 0; i--)
        //        {
        //            var firstCar = GetCortegeCar(row, (CortegeColumn)i);
        //            var secondCar = GetCortegeCar(row, (CortegeColumn)(i - 1));

        //            if (firstCar == null && secondCar != null)
        //            {
        //                secondCar.SetPoint(GetCortegePoint(row, (CortegeColumn)i));
        //            }
        //        }
        //    }
        //}

        private RaidController GetCortegeCar(CortegeRow row, CortegeColumn column)
        {
            return _raids.Find(e => e.CortegePoint.CortegeRow == row && e.CortegePoint.CortegeColumn == column);
        }
        #endregion

        #region METHODS PUBLIC
        public void DropAttack()
        {
            _isAttackLeft = false;
            _isAttackRight = false;
        }

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

                var raidController = raidCar.gameObject.GetComponent<GuardRaidController>();
                raidController.enabled = true;

                raidController.Initialize(raidSchema);
                raidController.Speed = _shiftSpeed;

                raidController.OnRam += Raid_OnRam;
                raidController.OnRaidDestroyed += Raid_OnRaidDestroyed;

                var healthBar = raidCar.GetComponentInChildren<Healthbar>(true);
                healthBar.gameObject.SetActive(true);

                CortegePoint point = _points.Find(e => e.CortegeRow == row && e.CortegeColumn == column);
                if (point != null)
                {
                    point.RaidController = raidController;

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

                var raidController = raidCar.GetComponent<LimoRaidController>();
                raidController.enabled = true;

                raidController.Initialize(raidSchema);
                raidController.Speed = _shiftSpeed;

                raidController.OnRaidDestroyed += Raid_OnRaidDestroyed;
                raidController.OnLimoDestroyed += Limo_OnLimoDestroyed;

                var healthBar = raidCar.GetComponentInChildren<Healthbar>(true);
                healthBar.gameObject.SetActive(true);

                CortegePoint point = _points.Find(e => e.CortegeRow == row && e.CortegeColumn == column);
                if (point != null)
                {
                    point.RaidController = raidController;

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
            
            Destroy(raid.gameObject);
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

            _cortege = new Cortege();

            CortegeCell cell;
            CortegePoint point;

            // front
            cell = _cortege.GetCellByPosition(CortegePosition.Front, CortegePosition.Left);
            point = _points.Find(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Two);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            cell = _cortege.GetCellByPosition(CortegePosition.Front, CortegePosition.Center);
            point = _points.Find(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Three);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            cell = _cortege.GetCellByPosition(CortegePosition.Front, CortegePosition.Right);
            point = _points.Find(e => e.CortegeRow == CortegeRow.Three && e.CortegeColumn == CortegeColumn.Four);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            // middle
            cell = _cortege.GetCellByPosition(CortegePosition.Middle, CortegePosition.Left);
            point = _points.Find(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Two);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            cell = _cortege.GetCellByPosition(CortegePosition.Middle, CortegePosition.Center);
            point = _points.Find(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Three);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            cell = _cortege.GetCellByPosition(CortegePosition.Middle, CortegePosition.Right);
            point = _points.Find(e => e.CortegeRow == CortegeRow.Two && e.CortegeColumn == CortegeColumn.Four);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            // back
            cell = _cortege.GetCellByPosition(CortegePosition.Back, CortegePosition.Left);
            point = _points.Find(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Two);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            cell = _cortege.GetCellByPosition(CortegePosition.Back, CortegePosition.Center);
            point = _points.Find(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Three);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);

            cell = _cortege.GetCellByPosition(CortegePosition.Back, CortegePosition.Right);
            point = _points.Find(e => e.CortegeRow == CortegeRow.One && e.CortegeColumn == CortegeColumn.Four);
            cell.Raid = point.RaidController;
            cell.SetPoint(point);
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

        public EnemyController GetEnemyByColumn(CortegeColumn column)
        {
            return _enemies.Find(e => e.CortegePoint.CortegeColumn == column);
        }

        public int GetCortegeLevel()
        {
            var cortegeLevel = 1;
            var levelCount = 8;
            var levelSum = 0;

            foreach (var raid in _raids)
            {
                if(raid is GuardRaidController guard)
                {
                    levelSum += (int)guard.Config.Level;
                }
            }
            
            if (levelSum > 0)
            {
                cortegeLevel = levelSum / levelCount;
            }

            return cortegeLevel;
        }
        #endregion

        #region COROUTINES
        IEnumerator SpawnSuicideEnemies()
        {
            var cortegeLevel = GetCortegeLevel();

            var timeDelay = 3f;
            while (true)
            {
                yield return new WaitForSeconds(timeDelay);

                var distance = Vector3.Distance(_startPosition, transform.position);
                var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
                var enemyName = $"Suicide0{cortegeLevel + enemyPowerUp}";

                var enemySchema = Resources.Load<SuicideEnemyScheme>(enemyName);
                var randomColumn = UnityEngine.Random.Range(1, 4);

                SetEnemy<SuicideEnemyScheme>((CortegeColumn)randomColumn, enemySchema);
            }
        }

        IEnumerator SpawnShootEnemies()
        {
            var cortegeLevel = GetCortegeLevel();

            var timeDelay = 5f;
            while (true)
            {
                yield return new WaitForSeconds(timeDelay);

                var distance = Vector3.Distance(_startPosition, transform.position);
                var enemyPowerUp = Mathf.FloorToInt(distance / _powerUpDistance);
                var enemyName = $"Shoot0{cortegeLevel + enemyPowerUp}";

                var enemySchema = Resources.Load<ShootEnemyScheme>(enemyName);
                var column = UnityEngine.Random.Range(0f, 1f) < 0.5f ? CortegeColumn.One : CortegeColumn.Five;
                
                var enemy = _enemies.Find(e => e.CortegePoint.CortegeColumn == column);
                var raid = _raids.Find(e => e.CortegePoint.CortegeColumn == column);
                if(enemy == null && raid == null)
                {
                    SetEnemy(column, enemySchema);
                }
            }
        }

        IEnumerator FalseAttack(bool isSideLeft)
        {
            if (isSideLeft)
            {
                _cortege.MoveLeft();
            }
            else
            {
                _cortege.MoveRight();
            }

            yield return new WaitForSeconds(0.2f);

            if (isSideLeft)
            {
                _cortege.MoveRight();
            }
            else
            {
                _cortege.MoveLeft();
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
