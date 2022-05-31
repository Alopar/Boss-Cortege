using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BossCortege
{
    public class HUDUiController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private Button _addCarButton;
        [SerializeField] private Button _goButton;
        [SerializeField] private Button _stopButton;
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            GameManager.Instance.OnCortegeStop += GameManager_OnCortegeStop;
            GameManager.Instance.OnMoneyChange += GameManager_OnMoneyChange;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnMoneyChange -= GameManager_OnMoneyChange;
        }
        #endregion

        #region HANDLERS
        private void GameManager_OnCortegeStop()
        {
            ChangeButtons(true);
        }

        private void GameManager_OnMoneyChange(int value)
        {
            _moneyText.text = $"Money: {value:d7}";
        }
        #endregion

        #region METHODS PRIVATE
        private void ChangeButtons(bool parking)
        {
            if (parking)
            {
                _addCarButton.gameObject.SetActive(true);
                _goButton.gameObject.SetActive(true);
                _stopButton.gameObject.SetActive(false);
            }
            else
            {
                _addCarButton.gameObject.SetActive(false);
                _goButton.gameObject.SetActive(false);
                _stopButton.gameObject.SetActive(true);
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void AddCar()
        {
            GameManager.Instance.BuyCar(100);
        }

        public void Go()
        {
            GameManager.Instance.GoCortege();
            ChangeButtons(false);
        }

        public void Stop()
        {
            GameManager.Instance.StopCortege();
        }
        #endregion
    }
}