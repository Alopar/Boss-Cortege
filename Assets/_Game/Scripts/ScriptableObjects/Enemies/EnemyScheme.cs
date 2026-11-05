using UnityEngine;

namespace BossCortege
{
    public abstract class EnemyScheme : ScriptableObject
    {
        #region FIELDS INSPECTOR
        [SerializeField] private uint _money;
        [SerializeField] private PowerLevel _level;
        [SerializeField] private AbstractEnemy _prefab;

        [Space(10)]
        [SerializeField] private Healthbar _healthBarPref;
        [SerializeField] private Smoker _smokePref;
        [SerializeField] private GameObject _explosionPref;
        #endregion

        #region PROPERTIES
        public uint Money => _money;
        public PowerLevel Level => _level;
        public AbstractEnemy Prefab => _prefab;
        public Healthbar HealthBarPrefab => _healthBarPref;
        public Smoker SmokePrefab => _smokePref;
        public GameObject ExplosionPrefab => _explosionPref;
        #endregion
    }
}