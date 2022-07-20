using UnityEngine;

namespace BossCortege
{
    public class AudioManager : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private AudioSource _source;
        #endregion

        #region FIELDS PRIVATE
        private static AudioManager _instance;
        #endregion

        #region PROPERTIES
        public static AudioManager Instance => _instance;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void PlaySound(string name)
        {
            var clip = Resources.Load<AudioClip>(name);
            if(clip == null)
            {
                print("Sound not found!");
                return;
            }

            _source.PlayOneShot(clip);
        }
        #endregion
    }
}
