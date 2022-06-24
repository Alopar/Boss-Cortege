using UnityEngine;


namespace BossCortege
{
    class BuildRaceGuard : IBuildCarStrategy
    {   
        private GuardScheme _scheme;
        private float _speed;

        public BuildRaceGuard(GuardScheme scheme, float speed)
        {   
            _scheme = scheme;
            _speed = speed;
        }

        public AbstractCar BuildCar()
        {
            var car = GameObject.Instantiate(_scheme.Prefab);

            car.gameObject.AddComponent<MoveComponent>().Init(_speed);
            car.gameObject.AddComponent<RamComponent>().Init(_scheme.Damage);
            car.gameObject.AddComponent<DieComponent>().Init(_scheme.ExplosionPrefab);

            var health = car.gameObject.AddComponent<HealthComponent>();
            health.Init(_scheme.Durability);

            var healthBar = GameObject.Instantiate(_scheme.HealthBarPrefab);
            healthBar.transform.SetParent(car.HealthPoint, false);
            healthBar.Init(health);

            var smoke = GameObject.Instantiate(_scheme.SmokePrefab);
            smoke.transform.SetParent(car.SmokePoint, false);
            smoke.Init(health);

            car.Init(_scheme, new RaceState());

            return car;
        }
    }
}
