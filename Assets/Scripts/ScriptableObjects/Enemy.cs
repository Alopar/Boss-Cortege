using UnityEngine;

namespace BossCortege
{
    public abstract class Enemy : ScriptableObject
    {
        #region FIELDS INSPECTOR
        [SerializeField] private EnemyController _enemyPrefab;
        #endregion

        #region PROPERTIES
        public EnemyController EnemyPrefab => _enemyPrefab;
        #endregion
    }
}
