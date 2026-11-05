using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class RaceComponent : MonoBehaviour
    {
        #region UNITY CALLBACKS
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "StopSpawn")
            {
                EventHolder<SpawnStopInfo>.NotifyListeners(null);
            }

            if (other.tag == "Gate")
            {
                EventHolder<RaceFinishInfo>.NotifyListeners(null);
            }
        }
        #endregion
    }
}
