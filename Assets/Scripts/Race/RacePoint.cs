using System.Linq;
using UnityEngine;

namespace BossCortege
{
    public class RacePoint : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField, Range(0, 4)] private uint _row;
        [SerializeField, Range(0, 4)] private uint _column;
        #endregion

        #region METHODS PRIVATE
        private RacePoint _topPoint;
        private RacePoint _leftPoint;
        private RacePoint _rightPoint;
        private RacePoint _bottomPoint;
        #endregion

        #region UNITY CALLBACKS
        private void Start()
        {
            GetNeighbourPoint();
        }
        #endregion

        #region METHODS PRIVATE
        private void GetNeighbourPoint()
        {
            var racePoints = FindObjectsOfType<RacePoint>().ToList();

            _topPoint = racePoints.Find(e => e.Row == _row + 1 && e.Column == _column);
            _leftPoint = racePoints.Find(e => e.Column == _column - 1 && e.Row == _row);
            _rightPoint = racePoints.Find(e => e.Column == _column + 1 && e.Row == _row);
            _bottomPoint = racePoints.Find(e => e.Row == _row - 1 && e.Column == _column);
        }
        #endregion

        #region PROPERTIES
        public uint Row => _row;
        public uint Column => _column;

        public RacePoint TopPoint => _topPoint;
        public RacePoint LeftPoint => _leftPoint;
        public RacePoint RightPoint => _rightPoint;
        public RacePoint BottomPoint => _bottomPoint;
        #endregion
    }
}
