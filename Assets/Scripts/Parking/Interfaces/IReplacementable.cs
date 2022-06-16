using System;
using System.Collections;
using UnityEngine;

namespace BossCortege
{
    public interface IReplacementable
    {
        public event Action OnReplaced;

        public AbstractPlace Replace();
        public void SetPlace(AbstractPlace place);
    }
}