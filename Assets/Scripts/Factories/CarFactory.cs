using UnityEngine;

namespace BossCortege
{
    public class CarFactory : ICarFactory
    {
        public IReplacementable CreateCar(ICarFactoryStrategy strategy)
        {
            return strategy.BuildCar();
        }
    }

    public interface ICarFactory
    {
        public IReplacementable CreateCar(ICarFactoryStrategy strategy);
    }

    public enum CarType
    {
        Boss,
        Guard
    }
}
