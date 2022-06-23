using UnityEngine;


namespace BossCortege
{
    class BuildShootEnemy : IBuildEnemyStrategy
    {   
        private ShootEnemyScheme _scheme;

        public BuildShootEnemy(ShootEnemyScheme scheme)
        {   
            _scheme = scheme;
        }

        public AbstractEnemy BuildEnemy()
        {
            var enemy = GameObject.Instantiate(_scheme.Prefab);

            enemy.gameObject.AddComponent<MoveComponent>().Init(_scheme.Speed);
            enemy.gameObject.AddComponent<HealthComponent>().Init(_scheme.Durability);
            enemy.gameObject.AddComponent<ShootComponent>().Init(_scheme.ShootDamage, _scheme.RateOfFire, _scheme.ProjectileScheme);
            enemy.gameObject.AddComponent<ShootDieComponent>().Init(_scheme.ExplosionPrefab);
            enemy.Init(_scheme);

            return enemy;
        }
    }
}
