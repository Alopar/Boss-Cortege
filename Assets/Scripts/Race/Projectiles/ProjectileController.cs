using UnityEngine;

namespace BossCortege
{
    public class ProjectileController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private float _speed;
        private uint _damage;
        private Vector3 _aimPoint;

        private GameObject _hitPrefab;

        private Rigidbody _rigidbody;
        #endregion

        #region PROPERTIES
        public uint Damage => _damage;
        #endregion

        #region UNITY CALLBACKS
        private void FixedUpdate()
        {
            var currentPosition = transform.position + (transform.forward * _speed * Time.deltaTime);
            _rigidbody.MovePosition(currentPosition);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy") return;

            var healthComponent = collision.gameObject.GetComponent<IDamageable>();
            if (healthComponent != null)
            {
                healthComponent.TrySetDamage(_damage, DamageType.Bullet);
                Hit();

                Destroy(gameObject);
            }
        }
        #endregion

        #region METHODS PRIVATE
        private void Hit()
        {
            if (_hitPrefab == null) return;
            Instantiate(_hitPrefab, transform.position, transform.rotation);
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(float speed, uint damage, GameObject hitPrefab, BossCar car)
        {
            _speed = speed;
            _damage = damage;
            _hitPrefab = hitPrefab;

            _aimPoint = car.transform.position;
            _aimPoint.z += 1f;
            transform.LookAt(_aimPoint);

            _rigidbody = GetComponent<Rigidbody>();

            Destroy(gameObject, 2f);
        }
        #endregion
    }
}