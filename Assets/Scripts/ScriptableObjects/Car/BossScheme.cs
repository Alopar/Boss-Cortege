using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewLimo", menuName = "Configs/Cars/Limo", order = -99)]
    public class BossScheme : CarScheme
    {
        #region FIELDS INSPECTOR
        [SerializeField] private BossCar _prefab;
        #endregion

        #region PROPERTIES
        public BossCar Prefab => _prefab;
        #endregion
    }
}