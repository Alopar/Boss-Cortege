using UnityEngine;

namespace BossCortege
{
    public class CortegePlace : Place
    {
        #region FIELDS INSPECTOR
        [SerializeField] private bool _isLimo;
        [SerializeField] private CortegeRow _row;
        [SerializeField] private CortegeColumn _column;
        #endregion

        #region PROPERTIES
        public bool IsLimo => _isLimo;
        public CortegeRow CortegeRow => _row;
        public CortegeColumn CortegeColumn => _column;
        #endregion
    }
}