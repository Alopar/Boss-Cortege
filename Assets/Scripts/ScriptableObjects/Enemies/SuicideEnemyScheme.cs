using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewSuicideEnemy", menuName = "Configs/Enemies/SuicideEnemy", order = -94)]
    public class SuicideEnemyScheme : EnemyScheme
    {
        #region FIELDS INSPECTOR
        [Space(10)]
        [SerializeField] private float _speed;

        [Space(10)]
        [SerializeField] private uint _ramDamage;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public uint RamDamage => _ramDamage;
        #endregion
    }
}
