using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class CortegeElem
    {
        #region FIELDS PRIVATE
        private AbstractCar _car;
        private RacePoint _point;
        private List<CortegeElem> _spareElem;

        private uint _row;
        private uint _column;

        public CortegeElem(AbstractCar car, uint row, uint column, RacePoint point)
        {
            _row = row;
            _column = column;
            _point = point;
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
        public AbstractCar Car => _car;
        #endregion

        #region HANDLERS
        private void Spare_OnCortegeElemDestroy(CortegeElem spare)
        {
            _spareElem.Remove(spare);
            spare.OnCortegeElemDestroy -= Spare_OnCortegeElemDestroy;
        }

        private void Car_OnCarDestroyed(AbstractCar car)
        {
            _car = null;
            car.OnCarDestroyed -= Car_OnCarDestroyed;

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
            _car.OnCarDestroyed += Car_OnCarDestroyed;
            _car.GetComponent<MoveComponent>().SetPoint(_point);
        }

        private bool TryChangeCar()
        {
            if (_spareElem.Count == 0) return false;

            var spareCar = _spareElem[0].ExtractCar();
            SetCar(spareCar);

            return true;
        }

        private void CarSetPoint(RacePoint point)
        {
            if (_car != null)
            {
                _car.GetComponent<MoveComponent>().SetPoint(point);
            }
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
            CarSetPoint(_point);
        }

        public void ShiftRight()
        {
            _point = _point.RightPoint;
            CarSetPoint(_point);
        }

        public AbstractCar ExtractCar()
        {
            var car = _car;
            _car = null;

            if (!TryChangeCar())
            {
                OnCortegeElemDestroy?.Invoke(this);
            }

            return car;
        }
        #endregion
    }
}