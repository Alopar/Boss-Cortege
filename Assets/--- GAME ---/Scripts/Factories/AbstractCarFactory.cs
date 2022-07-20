using UnityEngine;

namespace BossCortege
{
    //public abstract class AbstractCarFactory
    //{
    //    public abstract AbstractCar FactoryMethod(PowerLevel level);
    //}

    //public class GuardParkingFactory : AbstractCarFactory
    //{
    //    public override AbstractCar FactoryMethod(PowerLevel level)
    //    {
    //        string schemeName = $"Guard0{(int)level}";
    //        var guardScheme = Resources.Load<GuardScheme>(schemeName);
    //        var car = GameObject.Instantiate(guardScheme.Prefab);
    //        car.Init(guardScheme);
    //        car.gameObject.AddComponent<MergeComponent>();
    //        car.gameObject.AddComponent<PlaceComponent>();

    //        return car;
    //    }
    //}

    //public class GuardRaceFactory : AbstractCarFactory
    //{
    //    public override AbstractCar FactoryMethod(PowerLevel level)
    //    {
    //        string schemeName = $"Guard0{(int)level}";
    //        var guardScheme = Resources.Load<GuardScheme>(schemeName);
    //        var car = GameObject.Instantiate(guardScheme.Prefab);
    //        car.Init(guardScheme);
    //        car.gameObject.AddComponent<MergeComponent>();
    //        car.gameObject.AddComponent<PlaceComponent>();

    //        return car;
    //    }
    //}

    //public class BossParkingFactory : AbstractCarFactory
    //{
    //    public override AbstractCar FactoryMethod(PowerLevel level)
    //    {
    //        string schemeName = $"Limo0{(int)level}";
    //        var bossScheme = Resources.Load<BossScheme>(schemeName);
    //        var car = GameObject.Instantiate(bossScheme.Prefab);
    //        car.Init(bossScheme);
    //        car.gameObject.AddComponent<PlaceComponent>();

    //        return car;
    //    }
    //}
}