using UnityEngine;
using DG.Tweening;

namespace BossCortege
{
    public class BarrieController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _barrier;
        [SerializeField] private float _openSpeed;
        #endregion

        #region METHODS PUBLIC
        public void UpBarrier()
        {
            _barrier.DORotate(new Vector3(0, 0, -90), _openSpeed, RotateMode.Fast);
        }

        public void DownBarrier()
        {
            _barrier.DORotate(new Vector3(0, 0, -180), _openSpeed, RotateMode.Fast);
        }
        #endregion
    }
}
