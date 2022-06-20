using UnityEngine;


namespace BossCortege
{
    class GuardCarFactoryStrategy : ICarFactoryStrategy
    {
        private PowerLevel _level;

        public GuardCarFactoryStrategy(PowerLevel level)
        {
            _level = level;
        }

        public IReplacementable BuildCar()
        {
            string schemeName = $"Guard0{(int)_level}";
            var guardScheme = Resources.Load<GuardScheme>(schemeName);
            var car = GameObject.Instantiate(guardScheme.Prefab);
            car.Init(guardScheme);
            car.gameObject.AddComponent<MergeComponent>();
            var placeComponent = car.gameObject.AddComponent<PlaceComponent>();

            return placeComponent;
        }
    }
}
