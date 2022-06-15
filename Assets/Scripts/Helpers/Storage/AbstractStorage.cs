namespace BossCortege
{
    public abstract class AbstractStorage<T>
    {
        protected string _name;

        public AbstractStorage(string name)
        {
            _name = name;
        }

        public abstract void Save(T value);
        public abstract T Load();
    }
}