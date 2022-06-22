using UnityEngine;


namespace BossCortege
{
    class BuildParkingGuard : IBuildCarStrategy
    {
        private PowerLevel _level;

        public BuildParkingGuard(PowerLevel level)
        {
            _level = level;
        }

        public AbstractCar BuildCar()
        {
            string schemeName = $"Guard0{(int)_level}";
            var guardScheme = Resources.Load<GuardScheme>(schemeName);
            var car = GameObject.Instantiate(guardScheme.Prefab);
            car.SetScheme(guardScheme);

            var place = car.gameObject.AddComponent<PlaceComponent>();
            car.gameObject.AddComponent<MergeComponent>().Init(car, place);

            return car;
        }
    }
}
