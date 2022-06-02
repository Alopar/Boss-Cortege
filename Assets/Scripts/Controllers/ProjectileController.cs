using UnityEngine;

namespace BossCortege
{
    public class ProjectileController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private float _speed;
        private uint _damage;
        private Transform _aim;

        private bool _initialized = false;
        #endregion

        #region PROPERTIES
        public uint Damage => _damage;
        #endregion

        #region UNITY CALLBACKS
        private void OnDisable()
        {
            _initialized = false;
        }

        private void FixedUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, _aim.position, _speed * Time.fixedDeltaTime);
        }
        #endregion

        #region METHODS PUBLIC
        public void Initialize(float speed, uint damage, Transform aim)
        {
            if (_initialized) return;
            _initialized = true;

            _speed = speed;
            _damage = damage;
            
            _aim = aim;
            transform.LookAt(_aim);
        }
        #endregion
    }
}