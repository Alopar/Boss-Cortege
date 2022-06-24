using System;
using UnityEngine;
using DG.Tweening;

namespace BossCortege
{
    public class RaceState : ICarState
    {
        #region FIELDS PRIVATE
        private AbstractCar _car;
        private RamComponent _ram;
        private MoveComponent _move;
        private HealthComponent _health;
        private DieComponent _die;

        private Healthbar _healthbar;
        #endregion

        #region HANDLERS
        private void Health_OnDie()
        {
            GameObject.Destroy(_ram);
            GameObject.Destroy(_move);
            GameObject.Destroy(_health);
            GameObject.Destroy(_healthbar.gameObject);

            _die.Die();
            _car.Die();
        }

        private void Health_OnDamage(uint obj)
        {
            _car.Body.DOComplete();
            _car.Body.DOShakePosition(0.5f, new Vector3(0.2f, 0, 0), vibrato: 20);
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(AbstractCar car)
        {
            _car = car;

            _ram = _car.GetComponent<RamComponent>();
            _move = _car.GetComponent<MoveComponent>();

            _health = car.GetComponent<HealthComponent>();
            _health.OnDamage += Health_OnDamage;
            _health.OnDie += Health_OnDie;

            _healthbar = _car.GetComponentInChildren<Healthbar>();

            _die = car.GetComponent<DieComponent>();
        }
        #endregion
    }
}
