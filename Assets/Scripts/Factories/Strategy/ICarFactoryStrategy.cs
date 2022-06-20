namespace BossCortege
{
    public interface ICarFactoryStrategy
    {
        public IReplacementable BuildCar();
    }
}
