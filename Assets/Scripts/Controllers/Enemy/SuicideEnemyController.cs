using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public class SuicideEnemyController : EnemyController
    {
        #region FIELDS PRIVATE
        private SuicideEnemyScheme _config;

        private float _speed;
        private uint _ramMaxDamage;
        private uint _currentRamDamage;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Only the initial installation is available
        /// </summary>
        public SuicideEnemyScheme Config
        {
            get { return _config; }
            set
            {
                if (_config == null)
                {
                    _config = value;

                    _speed = _config.Speed;
                    _ramMaxDamage = _config.RamDamage;
                    _currentRamDamage = _ramMaxDamage;
                }
            }
        }
        public uint RamDamage => _currentRamDamage;
        #endregion

        #region UNITY CALLBACKS
        private void FixedUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.fixedDeltaTime);

            if (transform.position == _currentPoint.transform.position)
            {
                Die();
            }
        }
        #endregion
    }
}
