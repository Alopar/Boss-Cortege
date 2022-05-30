using UnityEngine;

namespace BossCortege
{
    public class ParkingPlace : Place
    {
        #region FIELDS INSPECTOR
        [SerializeField, Tooltip("Parking place order number")] private int _number = 0;
        #endregion

        #region PROPERTIES
        public int Number => _number;
        #endregion
    }
}
