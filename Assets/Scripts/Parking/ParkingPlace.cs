using UnityEngine;

namespace BossCortege
{
    public class ParkingPlace : Place
    {
        #region FIELDS INSPECTOR
        [SerializeField, Tooltip("Parking place order number"), Min(1)] private uint _number = 1;

        [Space(10)]
        [SerializeField] private Material _openPlaceMaterial;
        [SerializeField] private Material _closePlaceMaterial;
        #endregion

        #region FIELDS PRIVATE
        private bool _isOpen = false;
        private MeshRenderer _renderer;
        #endregion

        #region PROPERTIES
        public uint Number => _number;
        public override bool IsVacant => base.IsVacant && _isOpen;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material = _closePlaceMaterial;
        }
        #endregion

        #region METHODS PUBLIC
        public void Unlock()
        {
            _isOpen = true;
            _renderer.material = _openPlaceMaterial;
        }
        #endregion
    }
}
