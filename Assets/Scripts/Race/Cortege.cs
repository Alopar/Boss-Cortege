using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class Cortege
    {
        #region FIELDS PRIVATE
        private RaceManager _raceManager;

        private List<CortegeElem> _elems;

        public Cortege(RaceManager race)
        {
            _raceManager = race;
        }
        #endregion

        #region PROPERTIES

        #endregion

        #region HANDLERS
        #endregion


        #region METHODS PRIVATE
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
        public void Init(List<(AbstractCar car, CortegePlace place)> cars)
        {

        }
        #endregion
    }
}