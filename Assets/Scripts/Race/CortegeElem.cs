using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class CortegeElem
    {
        #region FIELDS PRIVATE
        private RacePoint _point;
        
        private AbstractCar _car;
        private MoveComponent _carMove;
        private HealthComponent _carHealth;

        private List<CortegeElem> _spareElem;

        private uint _row;
        private uint _column;

        public CortegeElem(AbstractCar car, uint row, uint column)
        {
            _row = row;
            _column = column;
            SetCar(car);
        }
        #endregion

        #region EVENTS
        public event Action<CortegeElem> OnCortegeElemDestroy;
        #endregion

        #region PROPERTIES
        public uint Row => _row;
        public uint Column => _column;
        public RacePoint Point => _point;
        #endregion

        #region HANDLERS
        private void Spare_OnCortegeElemDestroy(CortegeElem spare)
        {
            _spareElem.Remove(spare);
            spare.OnCortegeElemDestroy -= Spare_OnCortegeElemDestroy;
        }

        private void Car_OnDie()
        {
            _carHealth.OnDie -= Car_OnDie;

            if (!TryChangeCar())
            {
                OnCortegeElemDestroy?.Invoke(this);
            }
        }
        #endregion

        #region METHODS PRIVATE
        private void SetCar(AbstractCar car)
        {
            _car = car;
            _carMove = _car.GetComponent<MoveComponent>();
            _carHealth = _car.GetComponent<HealthComponent>();

            _carHealth.OnDie += Car_OnDie;
        }

        private bool TryChangeCar()
        {
            if (_spareElem.Count == 0) return false;

            var spareCar = _spareElem[0].ExtractCar();
            SetCar(spareCar);

            return true;
        }
        #endregion

        #region METHODS PUBLIC
        public void SetSpares(List<CortegeElem> spares)
        {
            _spareElem = spares;
            foreach (var spare in _spareElem)
            {
                spare.OnCortegeElemDestroy += Spare_OnCortegeElemDestroy;
            }
        }

        public void ShiftLeft()
        {
            _point = _point.LeftPoint;
            _carMove.SetPoint(_point);
        }

        public void ShiftRight()
        {
            _point = _point.RightPoint;
            _carMove.SetPoint(_point);
        }

        public AbstractCar ExtractCar()
        {
            var car = _car;

            if (!TryChangeCar())
            {
                OnCortegeElemDestroy?.Invoke(this);
            }

            return car;
        }
        #endregion
    }
}