using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class HUDUiController : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private GameObject _moneyBar;

        [Space(10)]
        [SerializeField] private TextMeshProUGUI _distanceText;
        [SerializeField] private GameObject _distanceBar;

        [Space(10)]
        [SerializeField] private Button _addCarButton;
        [SerializeField] private Button _addPlaceButton;

        [Space(10)]
        [SerializeField] private Button _goButton;
        [SerializeField] private Button _stopButton;
        #endregion

        #region HANDLERS
        private void StartRaceHandler(RaceStartInfo info)
        {
            EnableRaceHUD(true);
            EnableParkingHUD(false);
        }

        private void StopRaceHandler(RaceStopInfo info)
        {
            EnableRaceHUD(false);
            EnableParkingHUD(true);
        }

        private void MoneyChangeHandler(MoneyChangeInfo info)
        {
            _moneyText.text = info.Value.ToString();
        }

        private void DistanceChangeHandler(DistanceChangeInfo info)
        {
            _distanceText.text = info.Value.ToString();
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<MoneyChangeInfo>.AddListener(MoneyChangeHandler, true);
            EventHolder<DistanceChangeInfo>.AddListener(DistanceChangeHandler, true);
            EventHolder<RaceStartInfo>.AddListener(StartRaceHandler, false);
            EventHolder<RaceStopInfo>.AddListener(StopRaceHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<MoneyChangeInfo>.RemoveListener(MoneyChangeHandler);
            EventHolder<DistanceChangeInfo>.RemoveListener(DistanceChangeHandler);
            EventHolder<RaceStartInfo>.RemoveListener(StartRaceHandler);
            EventHolder<RaceStopInfo>.RemoveListener(StopRaceHandler);
        }

        private void Awake()
        {
            EnableRaceHUD(false);
            EnableParkingHUD(true);
        }
        #endregion

        #region METHODS PRIVATE
        private void EnableParkingHUD(bool on)
        {
            _addCarButton.gameObject.SetActive(on);
            _addPlaceButton.gameObject.SetActive(on);
            _goButton.gameObject.SetActive(on);
            _moneyBar.gameObject.SetActive(on);
        }

        private void EnableRaceHUD(bool on)
        {
            //_stopButton.gameObject.SetActive(on);
            _distanceBar.gameObject.SetActive(on);
        }
        #endregion

        #region METHODS PUBLIC
        public void AddCar()
        {
            EventHolder<BuyCarInfo>.NotifyListeners(new BuyCarInfo(100));
        }

        public void AddPlace()
        {
            EventHolder<BuyPlaceInfo>.NotifyListeners(new BuyPlaceInfo(1000));
        }

        public void Go()
        {
            EventHolder<RaceStartInfo>.NotifyListeners(null);
        }

        public void Stop()
        {
            EventHolder<RaceStopInfo>.NotifyListeners(null);
        }
        #endregion
    }
}