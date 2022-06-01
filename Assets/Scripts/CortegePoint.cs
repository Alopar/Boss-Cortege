using UnityEngine;

namespace BossCortege
{
    public class CortegePoint : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private CortegeRow _cortegeRow;
        [SerializeField] private CortegeColumn _cortegeColumn;

        [Space(10)]
        [SerializeField] private CortegePoint _topPoint;
        [SerializeField] private CortegePoint _leftPoint;
        [SerializeField] private CortegePoint _rightPoint;
        [SerializeField] private CortegePoint _bottomPoint;
        #endregion

        #region FIELDS PRIVATE
        private RaidController _raidController;
        #endregion

        #region PROPERTIES
        public CortegeRow CortegeRow => _cortegeRow;
        public CortegeColumn CortegeColumn => _cortegeColumn;
        public RaidController RaidController { get { return _raidController; } set { _raidController = value; } }

        public CortegePoint TopPoint => _topPoint;
        public CortegePoint LeftPoint => _leftPoint;
        public CortegePoint RightPoint => _rightPoint;
        public CortegePoint BottomPoint => _bottomPoint;
        #endregion
    }
}
