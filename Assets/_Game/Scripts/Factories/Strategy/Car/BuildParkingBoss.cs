using UnityEngine;

namespace BossCortege
{
    public class BuildParkingBoss : IBuildCarStrategy
    {
        public AbstractCar BuildCar()
        {
            var bossScheme = Resources.Load<BossScheme>("Limo01");
            var car = GameObject.Instantiate(bossScheme.Prefab);
            car.Init(bossScheme, new ParkingState());

            car.gameObject.AddComponent<PlaceComponent>();

            return car;
        }
    }
}