using UnityEngine;

namespace BossCortege
{
    public class BossCarFactoryStrategy : ICarFactoryStrategy
    {
        public IReplacementable BuildCar()
        {
            var bossScheme = Resources.Load<BossScheme>("Limo01");
            var car = GameObject.Instantiate(bossScheme.Prefab);
            car.Init(bossScheme);
            car.gameObject.AddComponent<MergeComponent>();
            var placeComponent = car.gameObject.AddComponent<PlaceComponent>();

            return placeComponent;
        }
    }
}