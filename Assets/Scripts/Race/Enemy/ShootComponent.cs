using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public class ShootComponent : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private uint _damage;
        private float _rateOfFire;
        private ProjectileSchema _projectileSchema;

        private Coroutine _fireCoroutine = null;

        private bool _isShooting = false;
        #endregion

        #region PROPERTIES
        public bool IsShooting => _isShooting;
        #endregion

        #region METHODS PUBLIC
        public void Init(uint damage, float rateOfFire, ProjectileSchema projectileSchema)
        {
            _damage = damage;
            _rateOfFire = rateOfFire;
            _projectileSchema = projectileSchema;
        }

        public void ShootOn()
        {
            if (_isShooting) return;

            _isShooting = true;
            _fireCoroutine = StartCoroutine(ShootTimer(_rateOfFire));
        }

        public void ShootOff()
        {
            _isShooting = false;
            StopCoroutine(_fireCoroutine);
        }
        #endregion

        #region COROUTINES
        IEnumerator ShootTimer(float delay)
        {
            while (true)
            {
                StartCoroutine(BursFire(3));

                yield return new WaitForSeconds(delay);
            }
        }

        IEnumerator BursFire(uint number)
        {
            var fireCounter = number;
            while(fireCounter > 0)
            {
                fireCounter--;
                var projectile = Instantiate(_projectileSchema.Prefab, transform.position, transform.rotation);
                projectile.Init(_projectileSchema.Speed, _damage, RaceManager.Instance.Boss);

                yield return new WaitForSeconds(0.05f);
            }
        }
        #endregion
    }
}
