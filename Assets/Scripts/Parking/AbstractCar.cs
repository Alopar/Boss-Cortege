using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public abstract class AbstractCar : MonoBehaviour
    {
        #region METHODS PUBLIC
        public abstract void Init(CarScheme scheme);
        #endregion
    }
}