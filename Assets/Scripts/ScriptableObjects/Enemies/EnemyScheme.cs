using UnityEngine;

namespace BossCortege
{
    public abstract class EnemyScheme : ScriptableObject
    {
        #region FIELDS INSPECTOR
        [SerializeField] private uint _money;
        [SerializeField] private PowerLevel _level;
        [SerializeField] private AbstractEnemy _prefab;
        #endregion

        #region PROPERTIES
        public uint Money => _money;
        public PowerLevel Level => _level;
        public AbstractEnemy Prefab => _prefab;
        #endregion
    }
}