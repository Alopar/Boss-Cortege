using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public class SuicideController : EnemyController
    {
        #region FIELDS PRIVATE
        private SuicideEnemy _settings;

        private float _speed;
        private uint _maxDamage;
        private uint _currentDamage;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Only the initial installation is available
        /// </summary>
        public SuicideEnemy Settings
        {
            get { return _settings; }
            set
            {
                if (_settings == null)
                {
                    _settings = value;

                    _speed = _settings.Speed;
                    _maxDamage = _settings.Damage;
                    _currentDamage = _maxDamage;
                }
            }
        }
        public uint Damage => _currentDamage;
        #endregion

        #region UNITY CALLBACKS
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.deltaTime);
            
            if(transform.position == _currentPoint.transform.position)
            {
                Die();
            }
        }
        #endregion

        #region METHODS PRIVATE
        #endregion

        #region METHODS PUBLIC
        public void SetPoint(CortegePoint point)
        {
            _currentPoint = point;
        }
        #endregion
    }
}
