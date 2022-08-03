using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class GameManager : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private CinemachineVirtualCamera _parkingCamera;
        [SerializeField] private CinemachineVirtualCamera _cortegeCamera;
        #endregion

        #region FIELDS PRIVATE
        private static GameManager _instance;

        private AbstractStorage<int> _currentLevelStorage;
        private MoneyDeposite _moneyWallet;
        private DistanceHolder _distance;
        #endregion

        #region PROPERTIES
        public static GameManager Instance => _instance;

        public int CurrentLevel => _currentLevelStorage.Load();
        public MoneyDeposite Wallet => _moneyWallet;
        public DistanceHolder Distance => _distance;
        #endregion

        #region HANDLERS
        private void StartRaceHandler(RaceStartInfo info)
        {
            _cortegeCamera.Priority = 30;
        }

        private void GoHomeHandler(GoHomeInfo info)
        {
            _cortegeCamera.Priority = 10;
        }

        private void NextLevelHandler(NextLevelInfo info)
        {
            _currentLevelStorage.Save(_currentLevelStorage.Load() + 1);
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<RaceStartInfo>.AddListener(StartRaceHandler, false);
            EventHolder<GoHomeInfo>.AddListener(GoHomeHandler, false);
            EventHolder<NextLevelInfo>.AddListener(NextLevelHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<RaceStartInfo>.RemoveListener(StartRaceHandler);
            EventHolder<GoHomeInfo>.RemoveListener(GoHomeHandler);
            EventHolder<NextLevelInfo>.RemoveListener(NextLevelHandler);
        }

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
                Init();
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("INITIALIZED") == 0)
            {
                _moneyWallet.SetCash(1000);
                _currentLevelStorage.Save(1);

                PlayerPrefs.SetInt("INITIALIZED", 1);
            }

            _moneyWallet.SetCash(0);
        }
        #endregion

        #region METHODS PRIVATE
        private void Init()
        {
            _distance = new DistanceHolder(new IntPlayerPrefStorage("BEST-DISTANCE"));
            _moneyWallet = new MoneyDeposite(new IntPlayerPrefStorage("MONEY"));
            _currentLevelStorage = new IntPlayerPrefStorage("CURRENT-LEVEL");

            if (!PlayerPrefs.HasKey("INITIALIZED"))
            {
                PlayerPrefs.SetInt("INITIALIZED", 0);
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void ClearGameStorage()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("INITIALIZED", 0);

            SceneManager.LoadScene(0);
        }
#if UNITY_EDITOR
        [ContextMenu("cheat: MORE MONEY")]
        public void MoreMoney()
        {
            _moneyWallet.SetCash(1000);
        }
#endif
        #endregion
    }
}