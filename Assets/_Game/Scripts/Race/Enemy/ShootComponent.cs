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
        private Turret _turret;

        private Coroutine _fireCoroutine = null;

        private bool _isShooting = false;
        #endregion

        #region PROPERTIES
        public bool IsShooting => _isShooting;
        #endregion

        #region METHODS PUBLIC
        public void Init(uint damage, float rateOfFire, ProjectileSchema projectileSchema, Turret turret)
        {
            _damage = damage;
            _rateOfFire = rateOfFire;
            _projectileSchema = projectileSchema;
            _turret = turret;
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
            _turret.FireOn();
            var fireCounter = number;
            while(fireCounter > 0)
            {
                fireCounter--;
                var projectile = Instantiate(_projectileSchema.Prefab, _turret.FirePoint.transform.position, transform.rotation);
                projectile.Init(_projectileSchema.Speed, _damage, _projectileSchema.HitPrefab, RaceManager.Instance.Boss);

                yield return new WaitForSeconds(0.05f);
            }
            _turret.FireOff();
        }
        #endregion
    }
}
