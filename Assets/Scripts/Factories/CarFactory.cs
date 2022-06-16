using UnityEngine;

namespace BossCortege
{
    public class CarFactory : ICarFactory
    {
        public AbstractCar CreateCar(ICarFactoryStrategy strategy)
        {
            return strategy.BuildCar();
        }
    }

    public interface ICarFactory
    {
        public AbstractCar CreateCar(ICarFactoryStrategy strategy);
    }

    public enum CarType
    {
        Boss,
        Guard
    }
}
