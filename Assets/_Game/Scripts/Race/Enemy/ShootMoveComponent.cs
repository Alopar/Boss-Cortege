using System;
using UnityEngine;
using DG.Tweening;

namespace BossCortege
{
    //public class ShootMoveComponent : MoveComponent
    //{
    //    #region UNITY CALLBACKS
    //    private void FixedUpdate()
    //    {
    //        var currentPosition = Vector3.MoveTowards(transform.position, _currentPoint.transform.position, _speed * 0.5f * Time.deltaTime);
    //        currentPosition += transform.forward * _speed * Time.deltaTime;
    //        _rigidbody.MovePosition(currentPosition);

    //        if (!_isPointReached)
    //        {
    //            if (Vector3.Distance(transform.position, _currentPoint.transform.position) < 0.1f)
    //            {
    //                _rigidbody.DORotate(new Vector3(0, 0, 0), 0.1f);

    //                OnPointReachedInvoke();

    //                _isPointReached = true;
    //            }
    //        }
    //    }
    //    #endregion

    //    #region METHODS PUBLIC
    //    public override void SetPoint(RacePoint point)
    //    {
    //        //if(_currentPoint != null)
    //        //{ 
    //        //    if (_currentPoint.transform.position.z > point.transform.position.z)
    //        //    {
    //        //        SetSpeed(Speed / 2);
    //        //    }
    //        //}

    //        _currentPoint = point;
    //        _isPointReached = false;
    //    }
    //    #endregion
    //}
}
