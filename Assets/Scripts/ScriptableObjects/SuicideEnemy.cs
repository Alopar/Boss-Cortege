using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewSuicideEnemy", menuName = "SuicideEnemy", order = 10)]
    public class SuicideEnemy : Enemy
    {
        #region FIELDS INSPECTOR
        [Space(10)]
        [SerializeField] private float _speed;
        [SerializeField] private uint _damage;

        [Space(10)]
        [SerializeField] private CarLevel _level;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public uint Damage => _damage;
        public CarLevel Level => _level;
        #endregion
    }
}
