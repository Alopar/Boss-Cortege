using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewBarricadeScheme", menuName = "Configs/Enemies/BarricadeScheme", order = -93)]
    public class BarricadeEnemyScheme : EnemyScheme
    {
        #region FIELDS INSPECTOR
        [Space(10)]
        [SerializeField] private uint _explosionDamage;
        #endregion

        #region PROPERTIES
        public uint ExplosionDamage => _explosionDamage;
        #endregion

        #region METHODS PUBLIC
        public override EnemyController Factory()
        {
            var enemy = Instantiate(Prefab) as BarricadeEnemyController;
            enemy.Config = this;

            return enemy;
        }
        #endregion
    }
}