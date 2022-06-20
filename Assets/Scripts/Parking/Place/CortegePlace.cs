using System.Collections.Generic;
using UnityEngine;

namespace BossCortege
{
    public class CortegePlace : AbstractPlace
    {
        #region FIELDS INSPECTOR
        [SerializeField] private bool _isBoss;
        [SerializeField, Range(0, 2)] private uint _row;
        [SerializeField, Range(0, 2)] private uint _column;
        [SerializeField] private List<CortegePlace> _spares;
        #endregion

        #region PROPERTIES
        public bool IsBoss => _isBoss;
        public uint Row => _row;
        public uint Column => _column;
        public List<CortegePlace> Spares => _spares;
        #endregion
    }
}