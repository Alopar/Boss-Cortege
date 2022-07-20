using System;
using UnityEngine;
using DG.Tweening;

namespace BossCortege
{
    public class MoveComponent : MonoBehaviour
    {
        #region FIELDS PRIVATE
        protected float _speed;
        protected RacePoint _currentPoint;
        protected bool _isPointReached;

        protected Rigidbody _rigidbody;
        #endregion

        #region PROPERTIES
        public float Speed => _speed;
        public RacePoint Point => _currentPoint;
        #endregion

        #region EVENTS
        public event Action OnPointReached;
        #endregion

        #region UNITY CALLBACKS
        private void FixedUpdate()
        {
            var currentPosition = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * 0.5f * Time.deltaTime);
            currentPosition += transform.forward * _speed * Time.deltaTime;
            _rigidbody.MovePosition(currentPosition);

            if (!_isPointReached)
            {
                if (Vector3.Distance(transform.position, _currentPoint.transform.position) < 0.1f)
                {
                    _rigidbody.DORotate(new Vector3(0, 0, 0), 0.1f);

                    OnPointReachedInvoke();

                    _isPointReached = true;
                }
            }
        }

        private void OnDestroy()
        {
            _rigidbody.DOKill();
        }
        #endregion

        #region METHODS PRIVATE
        protected void OnPointReachedInvoke()
        {
            OnPointReached?.Invoke();
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(float speed)
        {
            _speed = speed;
            _rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void SetPoint(RacePoint point)
        {
            if(_currentPoint != null)
            {
                if(_currentPoint.transform.position.x != point.transform.position.x)
                {
                    if (_currentPoint.transform.position.x > point.transform.position.x)
                    {
                        _rigidbody.DORotate(new Vector3(0, -7, 0), 0.2f);
                    }
                    else
                    {
                        _rigidbody.DORotate(new Vector3(0, 7, 0), 0.2f);
                    }
                }
            }

            _currentPoint = point;
            _isPointReached = false;
        }

        public void SetSpeed(float value)
        {
            _speed = value;
        }
        #endregion
    }
}
