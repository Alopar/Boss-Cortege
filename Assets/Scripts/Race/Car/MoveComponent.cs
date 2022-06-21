using System;
using UnityEngine;
using DG.Tweening;

namespace BossCortege
{
    public class MoveComponent : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private float _speed;
        private RacePoint _currentPoint;

        private Rigidbody _rigidbody;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public RacePoint Point => _currentPoint;
        #endregion

        #region EVENTS
        public event Action OnPointReached;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var currentPosition = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * Time.deltaTime);
            _rigidbody.MovePosition(currentPosition);

            if(transform.position == _currentPoint.transform.position)
            {
                transform.DORotate(new Vector3(0, 0, 0), 0.1f);

                OnPointReached?.Invoke();
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(float speed)
        {
            _speed = speed;
        }

        public void SetPoint(RacePoint point)
        {
            if(_currentPoint != null)
            {
                if(_currentPoint.transform.position.x != point.transform.position.x)
                {
                    if (_currentPoint.transform.position.x > point.transform.position.x)
                    {
                        transform.DORotate(new Vector3(0, -7, 0), 0.2f);
                    }
                    else
                    {
                        transform.DORotate(new Vector3(0, 7, 0), 0.2f);
                    }
                }
            }

            _currentPoint = point;
        }

        public void SetSpeed(float value)
        {
            _speed = value;
        }
        #endregion
    }
}
