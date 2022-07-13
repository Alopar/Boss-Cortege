using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class MenuUiController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GameObject _content;
        #endregion

        #region HANDLERS
        private void MenuOpenHandler(MenuOpenInfo info)
        {
            ShowPopup();
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<MenuOpenInfo>.AddListener(MenuOpenHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<MenuOpenInfo>.RemoveListener(MenuOpenHandler);
        }

        private void Awake()
        {
            _content.SetActive(false);
        }
        #endregion

        #region METHODS PUBLIC
        public void ClearProgress()
        {
            GameManager.Instance.ClearGameStorage();
        }

        public void ShowPopup()
        {
            _content.SetActive(true);
        }

        public void HidePopup()
        {
            _content.SetActive(false);
        }
        #endregion
    }
}
