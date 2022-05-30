using UnityEngine;

namespace BossCortege
{
    public class Bullet : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private float _speed;
        private uint _damage;
        #endregion

        #region PROPERTIES
        public float Speed { get { return _speed; } set { _speed = value; } }
        public uint Damage { get { return _damage; } set { _damage = value; } }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            transform.LookAt(CortegeController.Limo.transform);
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, CortegeController.Limo.transform.position, _speed * Time.deltaTime);
        }
        #endregion
    }
}
