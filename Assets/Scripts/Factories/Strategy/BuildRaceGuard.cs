﻿using UnityEngine;


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
            car.SetScheme(_scheme);

            car.gameObject.AddComponent<MoveComponent>().Init(_speed);
            car.gameObject.AddComponent<HealthComponent>().Init(_scheme.Durability);
            car.gameObject.AddComponent<RamComponent>().Init(_scheme.Damage);
            car.gameObject.AddComponent<DieComponent>().Init(_scheme.ExplosionPrefab);

            var healthBar = GameObject.Instantiate(_scheme.HealthBarPrefab);
            healthBar.transform.SetParent(car.HealthPoint, false);

            var smoke = GameObject.Instantiate(_scheme.SmokePrefab);
            smoke.transform.SetParent(car.SmokePoint, false);

            return car;
        }
    }
}
