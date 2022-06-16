using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class CortegeCell
    {
        #region FIELDS PRIVATE
        private RacePoint _point;
        private RaidController _raid;
        
        private CortegePosition _vertical;
        private CortegePosition _horizontal;

        public CortegeCell(CortegePosition vertical, CortegePosition horizontal)
        {
            _vertical = vertical;
            _horizontal = horizontal;
        }
        #endregion

        #region PROPERTIES
        public RaidController Raid { get { return _raid; } set { _raid = value; } }

        public RacePoint Point => _point;
        public CortegePosition Vertical => _vertical;
        public CortegePosition Horizontal => _horizontal;
        #endregion

        #region METHODS PUBLIC
        public void SetPoint(RacePoint point)
        {
            _point = point;

            if(_raid != null)
            {
                _raid.SetPoint(_point);
            }
        }
        #endregion
    }
}