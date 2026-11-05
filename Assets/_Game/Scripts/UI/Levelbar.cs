using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BossCortege
{
    public class Levelbar : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private TextMeshProUGUI _text;
        #endregion

        #region METHODS PUBLIC
        public void Init(PowerLevel level)
        {
            var number = (uint)level;
            _text.text = number.ToString();
        }
        #endregion
    }
}
