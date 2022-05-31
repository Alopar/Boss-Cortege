using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewSuicideEnemy", menuName = "Enemies/SuicideEnemy", order = 11)]
    public class SuicideEnemyScheme : EnemyScheme
    {
        #region FIELDS INSPECTOR
        [Space(10)]
        [SerializeField] private uint _ramDamage;
        #endregion

        #region PROPERTIES
        public uint RamDamage => _ramDamage;
        #endregion
    }
}
