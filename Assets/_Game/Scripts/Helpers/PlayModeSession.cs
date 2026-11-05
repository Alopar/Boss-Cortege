using UnityEngine;
using DG.Tweening;

namespace BossCortege
{
    static partial class PlayModeSession
    {
        [OnEnteringPlayMode]
        static void Begin()
        {
            DOTween.Clear(true);
        }
    }
}
