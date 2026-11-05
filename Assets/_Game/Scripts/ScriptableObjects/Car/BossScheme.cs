using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewBoss", menuName = "Configs/Cars/Boss", order = -99)]
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