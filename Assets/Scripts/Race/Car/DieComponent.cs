using UnityEngine;

namespace BossCortege
{
    public class DieComponent : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GameObject _explosionVfxPrefab;
        #endregion

        #region FIELDS PRIVATE
        protected Rigidbody _rigidbody;
        #endregion

        #region METHODS PRIVATE
        protected void ShowExplosion()
        {
            if (_explosionVfxPrefab == null) return;
            Instantiate(_explosionVfxPrefab, transform.position, transform.rotation);
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(GameObject explosionPrefab)
        {
            _explosionVfxPrefab = explosionPrefab;
            _rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void Die()
        {
            _rigidbody.isKinematic = false;

            _rigidbody.AddForce(transform.forward * 1000f, ForceMode.Impulse);
            _rigidbody.AddForce(transform.up * 800f, ForceMode.Impulse);

            var randomTorque = new Vector3(Random.value, Random.value, Random.value);
            _rigidbody.AddTorque(randomTorque * 200f, ForceMode.Impulse);

            Invoke(nameof(ShowExplosion), 1.4f);
            Destroy(gameObject, 1.5f);
        }
        #endregion
    }
}
