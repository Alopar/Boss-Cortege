using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BossCortege.EventHolder;
using Random = UnityEngine.Random;

namespace BossCortege
{
    public class Cortege : IDisposable
    {
        #region FIELDS PRIVATE
        private List<CortegeElem> _elems;
        private Vector2 _lastMoveDirection = Vector2.zero;

        public Cortege()
        {   
            EventHolder<InputSwipeInfo>.AddListener(InputSwipeHandler, false);
            EventHolder<RaceStopInfo>.AddListener(RaceStopHandler, false);
            EventHolder<RamInfo>.AddListener(RamHandler, false);
        }
        #endregion

        #region HANDLERS
        private void InputSwipeHandler(InputSwipeInfo info)
        {
            HandleInput(info.Direction);
        }

        private void RamHandler(RamInfo info)
        {
            _lastMoveDirection.x = _lastMoveDirection.x == 0 ? 0 : _lastMoveDirection.x < 0 ? 1 : -1;
            HandleInput(_lastMoveDirection);
        }

        private void RaceStopHandler(RaceStopInfo info)
        {
            //_elems.ForEach(e => GameObject.Destroy(e.Car.gameObject));
            _elems.ForEach(e => GameObject.Destroy(e.Car.gameObject.GetComponent<MoveComponent>()));
        }

        private void CortegeElem_OnCortegeElemDestroy(CortegeElem elem)
        {
            elem.OnCortegeElemDestroy -= CortegeElem_OnCortegeElemDestroy;
            _elems.Remove(elem);
        }
        #endregion

        #region METHODS PRIVATE
        private void HandleInput(Vector2 direction)
        {
            if (direction.x < 0)
            {
                var bound = _elems.OrderBy(e => e.Column).ToList().First();
                if (bound.Point.Column == 0) return;

                MoveLeft();

                _lastMoveDirection = direction;
            }

            if (direction.x > 0)
            {
                var bound = _elems.OrderByDescending(e => e.Column).ToList().First();
                if (bound.Point.Column == 4) return;

                MoveRight();

                _lastMoveDirection = direction;
            }
        }

        private void MoveLeft()
        {
            foreach (var elem in _elems)
            {
                elem.ShiftLeft();
            }
        }

        private void MoveRight()
        {
            foreach (var elem in _elems)
            {
                elem.ShiftRight();
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(List<CortegeCar> cortegeCars)
        {
            _elems = new List<CortegeElem>();

            foreach (var car in cortegeCars)
            {
                var cortegeElem = new CortegeElem(car.Car, car.Row, car.Column, car.RacePoint);
                cortegeElem.OnCortegeElemDestroy += CortegeElem_OnCortegeElemDestroy;

                _elems.Add(cortegeElem);
            }

            foreach (var car in cortegeCars)
            {
                var elem = _elems.Find(e => e.Row == car.Row && e.Column == car.Column);

                var spares = new List<CortegeElem>();
                foreach (var spare in car.Spares)
                {
                    var elemSpare = _elems.Find(e => e.Row == spare.Row && e.Column == spare.Column);

                    if(elemSpare != null)
                    {
                        spares.Add(elemSpare);
                    }
                }

                elem.SetSpares(spares);
            }
        }

        public List<RacePoint> GetOccupiedPoints()
        {
            var points = new List<RacePoint>();

            _elems.ForEach(e => points.Add(e.Point));

            return points;
        }

        public void Dispose()
        {
            _elems.ForEach(e => GameObject.Destroy(e.Car.gameObject));

            EventHolder<InputSwipeInfo>.RemoveListener(InputSwipeHandler);
            EventHolder<RaceStopInfo>.RemoveListener(RaceStopHandler);
            EventHolder<RamInfo>.RemoveListener(RamHandler);
        }
        #endregion
    }

    public struct CortegeCar
    {
        public AbstractCar Car;
        public uint Row;
        public uint Column;
        public RacePoint RacePoint;
        public List<CortegePlace> Spares;
    }
}