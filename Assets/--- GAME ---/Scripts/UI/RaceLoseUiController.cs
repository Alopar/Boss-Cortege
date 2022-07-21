using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BossCortege.EventHolder;
using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;

namespace BossCortege
{
    public class RaceLoseUiController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GameObject _content;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _currentDistanceText;
        [SerializeField] private TextMeshProUGUI _moneyDistanceText;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _enemiesText;
        [SerializeField] private TextMeshProUGUI _moneyEnemiesText;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _moreMoneyText;
        #endregion

        #region FIELDS PRIVATE
        private uint _currentMoney;
        private uint _moreMoney;
        #endregion

        #region HANDLERS
        private void RaceLoseHandler(RaceLoseInfo info)
        {
            _currentMoney = RaceManager.Instance.RaceMoney;
            _moreMoney = _currentMoney * 3;

            ShowPopup();
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<RaceLoseInfo>.AddListener(RaceLoseHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<RaceLoseInfo>.RemoveListener(RaceLoseHandler);
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
            _moneyDistanceText.text = RaceManager.Instance.DistanceMoney.ToString();

            _enemiesText.text = RaceManager.Instance.EnemyCount.ToString();
            _moneyEnemiesText.text = RaceManager.Instance.EnemyMoney.ToString();

            _moreMoneyText.text = _moreMoney.ToString();

            _content.SetActive(true);
        }

        private void HidePopup()
        {
            _content.SetActive(false);
        }
        #endregion

        #region METHODS PUBLIC
        public void TakeMoreMoney()
        {
            GameManager.Instance.Wallet.SetCash(_moreMoney);
            EventHolder<GoHomeInfo>.NotifyListeners(null);
            HidePopup();
        }

        public void Restart()
        {
            EventHolder<GoHomeInfo>.NotifyListeners(null);
            HidePopup();
        }
        #endregion
    }
}
