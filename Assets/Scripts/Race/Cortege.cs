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
            EventHolder<RamInfo>.AddListener(RamHandler, false);
        }
        #endregion

        #region PROPERTIES
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
        public void Init(List<(AbstractCar car, CortegePlace place)> cars_places)
        {
            foreach (var car_place in cars_places)
            {
                var cortegeElem = new CortegeElem(car_place.car, car_place.place.Row, car_place.place.Column);
                _elems.Add(cortegeElem);
            }

            foreach (var car_place in cars_places)
            {
                var elem = _elems.Find(e => e.Row == car_place.place.Row && e.Column == car_place.place.Column);

                var spares = new List<CortegeElem>();
                foreach (var spare in car_place.place.Sparses)
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

        public void Dispose()
        {
            EventHolder<InputSwipeInfo>.RemoveListener(InputSwipeHandler);
        }
        #endregion
    }
}