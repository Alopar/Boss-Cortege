using UnityEngine;

namespace BossCortege
{
    public abstract class AbstractPlace : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Transform _spawnPoint;
        #endregion

        #region FIELDS PRIVATE
        private IReplacementable _vechicle = null;
        #endregion

        #region PROPERTIES
        public bool IsEmpty => _vechicle == null;
        public virtual bool IsVacant => IsEmpty;

        public Transform SpawnPoint => _spawnPoint;
        public IReplacementable Vechicle => _vechicle;
        #endregion

        #region HANDLERS
        private void Vechicle_OnReplaced()
        {
            _vechicle.OnReplaced -= Vechicle_OnReplaced;
            _vechicle = null;
        }
        #endregion

        #region METHODS PUBLIC
        public bool TryPlaceVechicle(IReplacementable vechicle)
        {
            if (_vechicle != null) return false;

            _vechicle = vechicle;
            _vechicle.SetPlace(this);
            _vechicle.OnReplaced += Vechicle_OnReplaced;

            return true;
        }
        #endregion
    }
}
