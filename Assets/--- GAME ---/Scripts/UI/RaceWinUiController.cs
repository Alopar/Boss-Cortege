using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BossCortege.EventHolder;
using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;

namespace BossCortege
{
    public class RaceWinUiController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private GameObject _content;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _currentDistanceText;
        [SerializeField] private TextMeshProUGUI _moneyDistanceText;
        [SerializeField] private TextMeshProUGUI _bestDistanceText;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private TextMeshProUGUI _moneyLevelText;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _enemiesText;
        [SerializeField] private TextMeshProUGUI _moneyEnemiesText;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private TextMeshProUGUI _moreMoneyText;

        [Space(10)]
        [SerializeField] private List<UIParticle> _confettis;
        #endregion

        #region FIELDS PRIVATE
        private uint _currentMoney;
        private uint _moreMoney;
        #endregion

        #region HANDLERS
        private void RaceWinHandler(RaceWinInfo info)
        {
            _currentMoney = RaceManager.Instance.RaceMoney;
            _moreMoney = _currentMoney * 3;

            ShowPopup();
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<RaceWinInfo>.AddListener(RaceWinHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<RaceWinInfo>.RemoveListener(RaceWinHandler);
        }

        private void Awake()
        {
            _content.SetActive(false);
        }
        #endregion

        #region METHODS PRIVATE
        private void ShowPopup()
        {
            //_currentDistanceText.text = GameManager.Instance.Distance.CurrentDistance.ToString();
            //_moneyDistanceText.text = RaceManager.Instance.DistanceMoney.ToString();

            //_bestDistanceText.text = GameManager.Instance.Distance.BestDistance.ToString();

            _currentLevelText.text = GameManager.Instance.CurrentLevel.ToString();
            _moneyLevelText.text = (GameManager.Instance.CurrentLevel * 100).ToString();

            _enemiesText.text = RaceManager.Instance.EnemyCount.ToString();
            _moneyEnemiesText.text = RaceManager.Instance.EnemyMoney.ToString();

            _moneyText.text = _currentMoney.ToString();
            _moreMoneyText.text = _moreMoney.ToString();

            _content.SetActive(true);

            StartCoroutine(ShowConfetti());
        }

        private void HidePopup()
        {
            _content.SetActive(false);
            EventHolder<GoHomeInfo>.NotifyListeners(null);
            EventHolder<NextLevelInfo>.NotifyListeners(null);
        }
        #endregion

        #region METHODS PUBLIC
        public void TakeMoney()
        {
            GameManager.Instance.Wallet.SetCash(_currentMoney);
            HidePopup();
        }

        public void TakeMoreMoney()
        {
            GameManager.Instance.Wallet.SetCash(_moreMoney);
            HidePopup();
        }
        #endregion

        #region COROUTINES
        IEnumerator ShowConfetti()
        {
            for (int i = 0; i < _confettis.Count; i++)
            {
                _confettis[i].Play();
                yield return new WaitForSeconds(0.6f);
            }
        }
        #endregion
    }
}
