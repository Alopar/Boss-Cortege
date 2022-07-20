using UnityEngine;
using UnityEngine.UI;

namespace BossCortege
{
    public class ParkingPlace : AbstractPlace
    {
        #region FIELDS INSPECTOR
        [SerializeField, Tooltip("Parking place order number"), Min(1)] private uint _number = 1;

        [Space(10)]
        [SerializeField] private Image _closeIcon;
        #endregion

        #region FIELDS PRIVATE
        private bool _isOpen = false;
        #endregion

        #region PROPERTIES
        public uint Number => _number;
        public override bool IsVacant => base.IsVacant && _isOpen;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _closeIcon.enabled = true;
        }
        #endregion

        #region METHODS PUBLIC
        public void Unlock()
        {
            _isOpen = true;
            _closeIcon.enabled = false;
        }
        #endregion
    }
}
