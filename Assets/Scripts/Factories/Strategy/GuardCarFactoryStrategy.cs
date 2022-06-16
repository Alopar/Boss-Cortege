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

        public AbstractCar BuildCar()
        {
            string schemeName = $"Guard0{(int)_level}";
            var guardScheme = Resources.Load<GuardScheme>(schemeName);
            var car = GameObject.Instantiate(guardScheme.Prefab);
            car.Initialize(guardScheme);
            car.gameObject.AddComponent<Merger>();

            return car;
        }
    }
}
