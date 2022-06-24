using UnityEngine;

namespace BossCortege
{
    [SelectionBase]
    public class Turret : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _muzzel;
        #endregion

        #region METHODS PRIVATE
        private Transform _aim;
        #endregion

        #region PROPERTIES
        public Transform FirePoint => _firePoint;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _muzzel.SetActive(false);
        }

        private void FixedUpdate()
        {
            _body.transform.LookAt(_aim, Vector3.up);
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(Transform aim)
        {
            _aim = aim;
        }

        public void FireOn()
        {
            _muzzel.SetActive(true);
        }

        public void FireOff()
        {
            _muzzel.SetActive(false);
        }
        #endregion
    }
}
