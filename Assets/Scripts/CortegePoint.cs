using UnityEngine;

namespace BossCortege
{
    public class CortegePoint : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private CortegeRow _cortegeRow;
        [SerializeField] private CortegeColumn _cortegeColumn;
        #endregion

        #region FIELDS PRIVATE
        private RaidController _raidController;
        #endregion

        #region PROPERTIES
        public CortegeRow CortegeRow => _cortegeRow;
        public CortegeColumn CortegeColumn => _cortegeColumn;
        public RaidController RaidController { get { return _raidController; } set { _raidController = value; } }
        #endregion
    }
}
