using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class Cortege
    {
        #region FIELDS PRIVATE
        private List<CortegeCell> _cells;

        public Cortege()
        {
            _cells = new List<CortegeCell>();
            _cells.Add(new CortegeCell(CortegePosition.Front, CortegePosition.Left));
            _cells.Add(new CortegeCell(CortegePosition.Front, CortegePosition.Center));
            _cells.Add(new CortegeCell(CortegePosition.Front, CortegePosition.Right));
            _cells.Add(new CortegeCell(CortegePosition.Middle, CortegePosition.Left));
            _cells.Add(new CortegeCell(CortegePosition.Middle, CortegePosition.Center));
            _cells.Add(new CortegeCell(CortegePosition.Middle, CortegePosition.Right));
            _cells.Add(new CortegeCell(CortegePosition.Back, CortegePosition.Left));
            _cells.Add(new CortegeCell(CortegePosition.Back, CortegePosition.Center));
            _cells.Add(new CortegeCell(CortegePosition.Back, CortegePosition.Right));
        }
        #endregion

        #region PROPERTIES

        #endregion

        #region METHODS PUBLIC
        public CortegeCell GetCellByPosition(CortegePosition vertical, CortegePosition horizontal)
        {
            return _cells.Find(e => e.Vertical == vertical && e.Horizontal == horizontal);
        }

        public CortegeCell GetCellByRaid(RaidController raid)
        {
            return _cells.Find(e => e.Raid == raid);
        }

        public void MoveLeft()
        {
            foreach (var cell in _cells)
            {
                cell.SetPoint(cell.Point.LeftPoint);
            }
        }

        public void MoveRight()
        {
            foreach (var cell in _cells)
            {
                cell.SetPoint(cell.Point.RightPoint);
            }
        }
        #endregion
    }

    public enum CortegePosition
    {
        Front,
        Middle,
        Back,
        Left,
        Center,
        Right
    }
}