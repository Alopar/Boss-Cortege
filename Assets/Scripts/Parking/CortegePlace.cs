using UnityEngine;

namespace BossCortege
{
    public class CortegePlace : Place
    {
        #region FIELDS INSPECTOR
        [SerializeField] private bool _isBoss;
        [SerializeField] private CortegeRow _row;
        [SerializeField] private CortegeColumn _column;
        #endregion

        #region PROPERTIES
        public bool IsBoss => _isBoss;
        public CortegeRow CortegeRow => _row;
        public CortegeColumn CortegeColumn => _column;
        #endregion
    }
}