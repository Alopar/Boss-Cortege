using UnityEngine;

namespace BossCortege
{
    public class BossCarFactoryStrategy : ICarFactoryStrategy
    {
        public AbstractCar BuildCar()
        {
            var bossScheme = Resources.Load<BossScheme>("Limo01");
            var car = GameObject.Instantiate(bossScheme.Prefab);
            car.Initialize(bossScheme);

            return car;
        }
    }
}