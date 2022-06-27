using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace BossCortege
{
    public class ShootEnemy : AbstractEnemy
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _turretPoint;
        #endregion

        #region FIELDS PRIVATE
        private ShootEnemyScheme _scheme;

        private MoveComponent _move;
        private ShootComponent _shoot;
        private HealthComponent _health;
        private ShootDieComponent _die;

        private int _relocateCounter = 0;
        private bool _isLeaving = false;

        private Coroutine _relocateTimer;
        #endregion

        #region PROPERTIES
        public Transform TurretPoint => _turretPoint;
        public MoveComponent Move => _move;
        #endregion

        #region HANDLERS
        private void Health_OnDamage(uint damage, DamageType damageType)
        {
            if (damageType != DamageType.Ram) return;

            Body.DOComplete();
            Body.DOShakePosition(0.5f, new Vector3(0.2f, 0, 0), vibrato: 20);
        }

        private void Health_OnDie()
        {
            Die();
        }

        private void Move_OnPointReached()
        {
            if (_isLeaving)
            {
                Escape();
                return;
            }

            if (!_shoot.IsShooting)
            {
                _shoot.ShootOn();
            }

            if(_relocateTimer == null)
            {
                _relocateTimer = StartCoroutine(RelocateTimer(3f));
            }
        }
        #endregion

        #region METHODS PRIVATE
        private void Relocate()
        {
            int randomRow;
            while (true)
            {
                randomRow = Random.Range(1, 4);
                if (randomRow != _move.Point.Row) break;
            }
            
            var randomPoint = RaceManager.Instance.GetRacePoint((uint)randomRow, _move.Point.Column);
            _move.SetPoint(randomPoint);
        }

        private void Leave()
        {
            _isLeaving = true;
            _shoot.ShootOff();

            _move.SetSpeed(_move.Speed * 1.5f);
            _move.SetPoint(RaceManager.Instance.GetRacePoint(4, _move.Point.Column));
        }

        protected override void Die()
        {
            GameObject.Destroy(_move);
            GameObject.Destroy(_shoot);
            GameObject.Destroy(_health);

            base.Die();
            _die.Die();

            GameManager.Instance.Wallet.SetCash(_scheme.Money);
        }
        #endregion

        #region METHODS PUBLIC
        public override void Init(EnemyScheme scheme)
        {
            _scheme = scheme as ShootEnemyScheme;

            _health = GetComponent<HealthComponent>();
            _health.OnDamage += Health_OnDamage;
            _health.OnDie += Health_OnDie;

            _move = GetComponent<MoveComponent>();
            _move.OnPointReached += Move_OnPointReached;

            _shoot = GetComponent<ShootComponent>();

            _die = GetComponent<ShootDieComponent>();
        }
        #endregion

        #region COROUTINES
        IEnumerator RelocateTimer(float delay)
        {
            yield return new WaitForSeconds(delay);

            _relocateTimer = null;

            _relocateCounter++;
            if (_relocateCounter < _scheme.RelocateNumber)
            {
                Relocate();
            }
            else
            {
                Leave();
            }
        }
        #endregion
    }
}
