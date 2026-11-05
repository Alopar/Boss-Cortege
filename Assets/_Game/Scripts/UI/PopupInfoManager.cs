using UnityEngine;
using UnityEngine.UI;

namespace BossCortege
{
    public class PopupInfoManager : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private RectTransform _content;
        #endregion

        #region FIELDS PRIVATE
        private static PopupInfoManager _instance;
        #endregion

        #region PROPERTIES
        public static PopupInfoManager Instance => _instance;
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
        public void ShowPopupInfoText(PopupInfo popupPrefab, Vector3 position, string text)
        {
            var info = Instantiate(popupPrefab, _content);
            info.transform.position = position;
            info.SetText(text);
        }
        #endregion
    }
}
