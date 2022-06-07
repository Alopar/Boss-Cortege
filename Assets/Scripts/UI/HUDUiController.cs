using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BossCortege
{
    public class HUDUiController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _distanceText;

        [Space(10)]
        [SerializeField] private Button _addCarButton;
        [SerializeField] private Button _addPlaceButton;

        [Space(10)]
        [SerializeField] private Button _goButton;
        [SerializeField] private Button _stopButton;
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            GameManager.Instance.OnCortegeStop += GameManager_OnCortegeStop;
            GameManager.Instance.OnMoneyChange += GameManager_OnMoneyChange;
            GameManager.Instance.OnDistanceChange += GameManager_OnDistanceChange;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnMoneyChange -= GameManager_OnMoneyChange;
            GameManager.Instance.OnDistanceChange -= GameManager_OnDistanceChange;
        }
        #endregion

        #region HANDLERS
        private void GameManager_OnCortegeStop()
        {
            ChangeButtons(true);
        }

        private void GameManager_OnMoneyChange(int value)
        {
            _moneyText.text = $"Money: {value}";
        }

        private void GameManager_OnDistanceChange(int value)
        {
            _distanceText.text = $"Distance: {value}";
        }
        #endregion

        #region METHODS PRIVATE
        private void ChangeButtons(bool parking)
        {
            if (parking)
            {
                _addCarButton.gameObject.SetActive(true);
                _addPlaceButton.gameObject.SetActive(true);
                
                _goButton.gameObject.SetActive(true);
                _stopButton.gameObject.SetActive(false);
            }
            else
            {
                _addCarButton.gameObject.SetActive(false);
                _addPlaceButton.gameObject.SetActive(false);

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

        public void AddPlace()
        {
            GameManager.Instance.BuyPlace(1000);
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