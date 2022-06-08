using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewLimo", menuName = "Configs/Cars/Limo", order = -99)]
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