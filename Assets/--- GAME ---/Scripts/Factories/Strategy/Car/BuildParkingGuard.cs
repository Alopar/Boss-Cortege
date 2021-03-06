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
            string schemeNumber = (int)_level < 10 ? "0" + (int)_level : _level.ToString();
            string schemeName = $"Guard{schemeNumber}";
            var guardScheme = Resources.Load<GuardScheme>(schemeName);
            var car = GameObject.Instantiate(guardScheme.Prefab);
            car.Init(guardScheme, new ParkingState());

            var place = car.gameObject.AddComponent<PlaceComponent>();
            car.gameObject.AddComponent<MergeComponent>().Init(car, place);

            var levelbarPrefab = Resources.Load<Levelbar>("Levelbar");
            var levelbar = GameObject.Instantiate(levelbarPrefab);
            levelbar.transform.SetParent(car.LevelPoint, false);
            levelbar.Init(guardScheme.Level);

            return car;
        }
    }
}
