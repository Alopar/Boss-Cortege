using UnityEngine;


namespace BossCortege
{
    class BuildSuicideEnemy : IBuildEnemyStrategy
    {   
        private SuicideEnemyScheme _scheme;

        public BuildSuicideEnemy(SuicideEnemyScheme scheme)
        {   
            _scheme = scheme;
        }

        public AbstractEnemy BuildEnemy()
        {
            var enemy = GameObject.Instantiate(_scheme.Prefab);

            enemy.gameObject.AddComponent<MoveComponent>().Init(_scheme.Speed);
            enemy.gameObject.AddComponent<SuicideComponent>().Init(_scheme.RamDamage);
            enemy.gameObject.AddComponent<SuicideDieComponent>().Init(_scheme.ExplosionPrefab);
            enemy.Init(_scheme);

            return enemy;
        }
    }
}
