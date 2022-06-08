using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class BarricadeEnemyController : EnemyController
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GameObject _explosionVfxPrefab;
        #endregion

        #region FIELDS PRIVATE
        private BarricadeEnemyScheme _config;

        private uint _explosionDamage;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Only the initial installation is available
        /// </summary>
        public BarricadeEnemyScheme Config
        {
            get { return _config; }
            set
            {
                if (_config == null)
                {
                    _config = value;

                    _explosionDamage = _config.ExplosionDamage;
                }
            }
        }
        public uint ExplosionDamage => _explosionDamage;
        #endregion

        #region UNITY CALLBACKS
        private void FixedUpdate()
        {
            if (transform.position.z <= _currentPoint.transform.position.z)
            {
                Die();
            }
        }
        #endregion

        #region METHODS PRIVATE
        private void ShowExplosion()
        {
            Instantiate(_explosionVfxPrefab, transform.position, transform.rotation);
        }
        #endregion

        #region METHODS PUBLIC
        public override void Die()
        {
            if (IsDie) return;

            base.Die();

            ShowExplosion();
            Destroy(gameObject);
        }
        #endregion

    }
}