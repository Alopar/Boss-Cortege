using UnityEngine;

namespace BossCortege
{
    [CreateAssetMenu(fileName = "NewCar", menuName = "Car", order = 0)]
    public class Car : ScriptableObject
    {
        #region FIELDS INSPECTOR
        [SerializeField] private string _name;
        [SerializeField] private uint _cost;
        [SerializeField] private uint _durability;
        [SerializeField, Tooltip("Сила тарана")] private uint _damage;

        [Space(10)]
        [SerializeField] private CarLevel _level;
        [SerializeField] private CarController _carPrefab;
        #endregion

        #region PROPERTIES
        public string Name => _name;
        public uint Cost => _cost;
        public uint Durability => _durability;
        public uint Damage => _damage;
        public CarLevel Level => _level;
        public CarController CarPrefab => _carPrefab;
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
