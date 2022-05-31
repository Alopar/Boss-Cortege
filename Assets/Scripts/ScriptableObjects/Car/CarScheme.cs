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
        Level01,
        Level02,
        Level03,
        Level04,
        Level05,
        Level06,
        Level07,
        Level08,
        Level09,
        Level10,
        Level11,
        Level12,
        Level13,
        Level14,
        Level15,
        Level16,
        Level17,
        Level18,
        Level19,
        Level20
    }
}
