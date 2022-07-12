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
