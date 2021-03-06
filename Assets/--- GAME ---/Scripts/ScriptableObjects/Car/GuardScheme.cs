using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewGuard", menuName = "Configs/Cars/Guard", order = -98)]
    public class GuardScheme : CarScheme
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GuardCar _prefab;

        [Space(10)]
        [SerializeField, Tooltip("Сила тарана")] private uint _ramDamage;

        [Space(10)]
        [SerializeField] private PowerLevel _level;
        #endregion

        #region PROPERTIES
        public uint Damage => _ramDamage;
        public PowerLevel Level => _level;
        public GuardCar Prefab => _prefab;
        #endregion
    }
}