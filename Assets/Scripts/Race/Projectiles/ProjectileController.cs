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
            var currentPosition = Vector3.MoveTowards(transform.position, _aimPoint, _speed * Time.deltaTime);
            _rigidbody.MovePosition(currentPosition);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            var healthComponent = collision.gameObject.GetComponent<IDamageable>();
            if (healthComponent != null)
            {
                healthComponent.SetDamage(_damage);
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
        public void Init(float speed, uint damage, Transform aim)
        {
            _speed = speed;
            _damage = damage;

            _aimPoint = aim.position;
            _aimPoint.z += 1f;

            transform.LookAt(aim);
            Destroy(gameObject, 2f);
        }
        #endregion
    }
}