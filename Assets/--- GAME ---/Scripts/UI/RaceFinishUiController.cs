using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class RaceFinishUiController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GameObject _content;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _currentDistanceText;
        [SerializeField] private TextMeshProUGUI _moneyDistanceText;
        [SerializeField] private TextMeshProUGUI _bestDistanceText;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _enemiesText;
        [SerializeField] private TextMeshProUGUI _moneyEnemiesText;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _moreMoneyText;
        #endregion

        #region FIELDS PRIVATE
        private uint _currentMoney;
        private uint _moreMoney;
        #endregion

        #region HANDLERS
        private void RaceStopHandler(RaceStopInfo info)
        {
            _currentMoney = RaceManager.Instance.RaceMoney;
            _moreMoney = _currentMoney * 3;

            ShowPopup();
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<RaceStopInfo>.AddListener(RaceStopHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<RaceStopInfo>.RemoveListener(RaceStopHandler);
        }

        private void Awake()
        {
            _content.SetActive(false);
        }
        #endregion

        #region METHODS PRIVATE
        private void ShowPopup()
        {
            _currentDistanceText.text = GameManager.Instance.Distance.CurrentDistance.ToString();
            _moneyDistanceText.text = "+" + RaceManager.Instance.DistanceMoney.ToString();

            _bestDistanceText.text = GameManager.Instance.Distance.BestDistance.ToString();

            _enemiesText.text = RaceManager.Instance.EnemyCount.ToString();
            _moneyEnemiesText.text = "+" + RaceManager.Instance.EnemyMoney.ToString();

            _moneyText.text = "+" + _currentMoney.ToString();
            _moreMoneyText.text = "+" + _moreMoney.ToString();

            _content.SetActive(true);
        }

        private void HidePopup()
        {
            _content.SetActive(false);
        }
        #endregion

        #region METHODS PUBLIC
        public void TakeMoney()
        {
            GameManager.Instance.Wallet.SetCash(_currentMoney);
            EventHolder<GoHomeInfo>.NotifyListeners(null);
            HidePopup();
        }

        public void TakeMoreMoney()
        {
            GameManager.Instance.Wallet.SetCash(_moreMoney);
            EventHolder<GoHomeInfo>.NotifyListeners(null);
            HidePopup();
        }
        #endregion
    }
}
