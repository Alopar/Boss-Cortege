using UnityEngine;

namespace BossCortege
{
    class IntPlayerPrefStorage : AbstractStorage<int>
    {
        public IntPlayerPrefStorage(string name) : base(name)
        {
            if (!PlayerPrefs.HasKey(name))
            {
                PlayerPrefs.SetInt(name, 0);
            }
        }

        public override void Save(int value)
        {
            PlayerPrefs.SetInt(_name, value);
        }

        public override int Load()
        {
            return PlayerPrefs.GetInt(_name);
        }
    }
}
