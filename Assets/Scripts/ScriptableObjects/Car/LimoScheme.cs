using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewLimo", menuName = "Cars/Limo", order = 21)]
    public class LimoScheme : CarScheme
    {
        #region FIELDS INSPECTOR
        [SerializeField] private LimoParkingController _prefab;
        #endregion

        #region PROPERTIES
        public LimoParkingController Prefab => _prefab;
        #endregion
    }
}