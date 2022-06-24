using UnityEngine;


namespace BossCortege
{
    class BuildShootEnemy : IBuildEnemyStrategy
    {   
        private ShootEnemyScheme _scheme;
        private float _speed;

        public BuildShootEnemy(ShootEnemyScheme scheme, float speed)
        {   
            _scheme = scheme;
            _speed = speed;
        }

        public AbstractEnemy BuildEnemy()
        {
            var enemy = GameObject.Instantiate(_scheme.Prefab) as ShootEnemy;

            enemy.gameObject.AddComponent<MoveComponent>().Init(_speed);
            enemy.gameObject.AddComponent<ShootDieComponent>().Init(_scheme.ExplosionPrefab);

            var health = enemy.gameObject.AddComponent<HealthComponent>();
            health.Init(_scheme.Durability);

            var healthBar = GameObject.Instantiate(_scheme.HealthBarPrefab);
            healthBar.transform.SetParent(enemy.HealthPoint, false);
            healthBar.Init(health);

            var smoke = GameObject.Instantiate(_scheme.SmokePrefab);
            smoke.transform.SetParent(enemy.SmokePoint, false);
            smoke.Init(health);

            var turret = GameObject.Instantiate(_scheme.TurretPrefab);
            turret.transform.SetParent(enemy.TurretPoint, false);
            turret.Init(RaceManager.Instance.Boss.transform);
            enemy.gameObject.AddComponent<ShootComponent>().Init(_scheme.ShootDamage, _scheme.RateOfFire, _scheme.ProjectileScheme, turret);

            enemy.Init(_scheme);

            return enemy;
        }
    }
}
