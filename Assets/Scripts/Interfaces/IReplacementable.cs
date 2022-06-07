using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public interface IReplacementable
    {
        public event Action OnReplaced;
        public void SetPlace(Place place);
        public ParkingController GetCar();
    }
}