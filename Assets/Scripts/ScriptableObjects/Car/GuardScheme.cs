using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewGuard", menuName = "Cars/Guard", order = 20)]
    public class GuardScheme : CarScheme
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GuardParkingController _prefab;

        [Space(10)]
        [SerializeField, Tooltip("Сила тарана")] private uint _ramDamage;

        [Space(10)]
        [SerializeField] private CarLevel _level;
        #endregion

        #region PROPERTIES
        public uint Damage => _ramDamage;
        public CarLevel Level => _level;
        public GuardParkingController Prefab => _prefab;
        #endregion
    }
}