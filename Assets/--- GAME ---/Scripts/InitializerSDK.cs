using UnityEngine;
using GameAnalyticsSDK;

namespace BossCortege
{
    public static class InitializerSDK
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void GameAnalyticsSDKInstantiate()
        {
            var gameAnalytics = Resources.Load("GameAnalytics");
            GameObject.Instantiate(gameAnalytics);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void GameAnalyticsSDKInitialize()
        {
            GameAnalytics.Initialize();
        }
    }
}
