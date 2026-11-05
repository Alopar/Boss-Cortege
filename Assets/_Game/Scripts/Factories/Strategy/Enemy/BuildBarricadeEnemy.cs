using UnityEngine;


namespace BossCortege
{
    class BuildBarricadeEnemy : IBuildEnemyStrategy
    {   
        private BarricadeEnemyScheme _scheme;

        public BuildBarricadeEnemy(BarricadeEnemyScheme scheme)
        {   
            _scheme = scheme;
        }

        public AbstractEnemy BuildEnemy()
        {
            var enemy = GameObject.Instantiate(_scheme.Prefab);

            enemy.gameObject.AddComponent<ExplosionComponent>().Init(_scheme.ExplosionDamage);
            enemy.gameObject.AddComponent<BarricadeDieComponent>().Init(_scheme.ExplosionPrefab);
            enemy.Init(_scheme);

            return enemy;
        }
    }
}
