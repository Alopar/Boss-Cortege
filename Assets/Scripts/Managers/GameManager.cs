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

        private MoneyDeposite _moneyWallet;
        private DistanceHolder _distance;
        #endregion

        #region PROPERTIES
        public static GameManager Instance => _instance;
        
        public MoneyDeposite Wallet => _moneyWallet;
        public DistanceHolder Distance => _distance;
        #endregion

        #region HANDLERS
        public void StartRaceHandler(RaceStartInfo info)
        {
            _cortegeCamera.Priority = 30;
        }

        public void StopRaceHandler(RaceStopInfo info)
        {
            _cortegeCamera.Priority = 10;
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EventHolder<RaceStartInfo>.AddListener(StartRaceHandler, false);
            EventHolder<RaceStopInfo>.AddListener(StopRaceHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<RaceStartInfo>.RemoveListener(StartRaceHandler);
            EventHolder<RaceStopInfo>.RemoveListener(StopRaceHandler);
        }

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
                _moneyWallet = new MoneyDeposite(new IntPlayerPrefStorage("MONEY"));
                _distance = new DistanceHolder(new IntPlayerPrefStorage("BEST-DISTANCE"));
    }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey("INITIALIZED"))
            {
                _moneyWallet.SetCash(1000);
                PlayerPrefs.SetInt("INITIALIZED", 1);
            }

            _moneyWallet.SetCash(0);
        }
        #endregion

        #region METHODS PUBLIC
        public void ClearGameStorage()
        {
            PlayerPrefs.DeleteAll();
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