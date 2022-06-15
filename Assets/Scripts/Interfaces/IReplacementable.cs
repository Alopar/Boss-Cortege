using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public interface IReplacementable
    {
        public event Action OnReplaced;

        public Place Replace();
        public void SetPlace(Place place);
    }
}