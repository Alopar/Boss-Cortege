using UnityEngine;

namespace BossCortege
{
    public class ProjectileController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private float _speed;
        private uint _damage;
        private Vector3 _aimPoint;

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
                healthComponent.TrySetDamage(_damage);
                Destroy(gameObject);
                Hit();
            }
        }
        #endregion

        #region METHODS PRIVATE
        private void Hit()
        {
            //TODO: show vfx
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(float speed, uint damage, BossCar car)
        {
            _speed = speed;
            _damage = damage;

            _aimPoint = car.transform.position;
            _aimPoint.z += 1f;
            transform.LookAt(_aimPoint);

            _rigidbody = GetComponent<Rigidbody>();

            Destroy(gameObject, 2f);
        }
        #endregion
    }
}