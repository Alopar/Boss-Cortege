using UnityEngine;

namespace BossCortege
{   
    public abstract class CarScheme : ScriptableObject
    {
        #region FIELDS INSPECTOR
        [SerializeField] private string _name;
        [SerializeField] private uint _durability;
        #endregion

        #region PROPERTIES
        public string Name => _name;
        public uint Durability => _durability;
        #endregion
    }

    public enum CarLevel
    {
        Level01 = 1,
        Level02 = 2,
        Level03 = 3,
        Level04 = 4,
        Level05 = 5,
        Level06 = 6,
        Level07 = 7,
        Level08 = 8,
        Level09 = 9,
        Level10 = 10,
        Level11 = 11,
        Level12 = 12,
        Level13 = 13,
        Level14 = 14,
        Level15 = 15,
        Level16 = 16,
        Level17 = 17,
        Level18 = 18,
        Level19 = 19,
        Level20 = 20
    }
}
