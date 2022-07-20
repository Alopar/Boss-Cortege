using UnityEngine;

namespace BossCortege
{
    class StringPlayerPrefStorage : AbstractStorage<string>
    {
        public StringPlayerPrefStorage(string name) : base(name)
        {
            if (!PlayerPrefs.HasKey(name))
            {
                PlayerPrefs.SetInt(name, 0);
            }
        }

        public override void Save(string value)
        {
            PlayerPrefs.SetString(_name, value);
        }

        public override string Load()
        {
            return PlayerPrefs.GetString(_name);
        }
    }
}
