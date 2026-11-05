using System.Collections.Generic;
using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class FinishVFXController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private List<ParticleSystem> _confettis;
        #endregion

        #region HANDLERS
        private void RaceFinishHandler(RaceFinishInfo info)
        {
            ShowConfettis();
        }

        private void GoHoneHandler(GoHomeInfo info)
        {
            HideConfettis();
        }
        #endregion

        #region UNITY CALLBACKS
        private void Start()
        {
            HideConfettis();
        }

        private void OnEnable()
        {
            EventHolder<RaceFinishInfo>.AddListener(RaceFinishHandler, false);
            EventHolder<GoHomeInfo>.AddListener(GoHoneHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<RaceFinishInfo>.RemoveListener(RaceFinishHandler);
            EventHolder<GoHomeInfo>.RemoveListener(GoHoneHandler);
        }
        #endregion

        #region METHODS PRIVATE
        private void ShowConfettis()
        {
            foreach (var confetti in _confettis)
            {
                confetti.Play();
            }
        }

        private void HideConfettis()
        {
            foreach (var confetti in _confettis)
            {
                confetti.Stop();
            }
        }
        #endregion
    }
}
