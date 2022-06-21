using UnityEngine;

namespace BossCortege
{
    public class SuicideDieComponent : DieComponent
    {
        #region METHODS PUBLIC
        public override void Die()
        {
            _rigidbody.isKinematic = false;

            _rigidbody.AddForce(-transform.forward * 1000f, ForceMode.Impulse);
            _rigidbody.AddForce(transform.up * 800f, ForceMode.Impulse);

            var randomTorque = new Vector3(Random.value, Random.value, Random.value);
            _rigidbody.AddTorque(randomTorque * 200f, ForceMode.Impulse);

            Invoke(nameof(ShowExplosion), 1.4f);
            Destroy(gameObject, 1.5f);
        }
        #endregion
    }
}
