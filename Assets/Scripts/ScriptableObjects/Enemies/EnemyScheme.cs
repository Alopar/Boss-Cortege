using UnityEngine;

namespace BossCortege
{
    public abstract class EnemyScheme : ScriptableObject
    {
        #region FIELDS INSPECTOR
        [SerializeField] private uint _money;
        [SerializeField] private float _speed;        
        [SerializeField] private EnemyController _prefab;
        #endregion

        #region PROPERTIES
        public uint Money => _money;
        public float Speed => _speed;
        public EnemyController Prefab => _prefab;
        #endregion
    }
}