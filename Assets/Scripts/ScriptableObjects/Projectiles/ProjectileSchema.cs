using System.Collections;
using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "Projectiles/Projectile", order = 20)]
    public class ProjectileSchema : ScriptableObject
    {
        #region FIELDS INSPECTOR
        [SerializeField] private float _speed;
        [SerializeField] private ProjectileController _prefab;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public ProjectileController Prefab => _prefab;
        #endregion
    }
}