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
            if (IsDie) return;

            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.fixedDeltaTime);

            if (transform.position == _currentPoint.transform.position)
            {
                Die();
            }
        }
        #endregion

        #region METHODS PUBLIC
        public override void Die()
        {
            if (IsDie) return;

            base.Die();

            _rigidbody.isKinematic = false;

            _rigidbody.AddForce(-transform.forward * 1000f, ForceMode.Impulse);
            _rigidbody.AddForce(transform.up * 800f, ForceMode.Impulse);

            var randomTorque = new Vector3(Random.value, Random.value, Random.value);
            _rigidbody.AddTorque(randomTorque * 200f, ForceMode.Impulse);

            Destroy(gameObject, 1.5f);
        }
        #endregion
    }
}
