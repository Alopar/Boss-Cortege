using UnityEngine;
using UnityEngine.UI;

namespace BossCortege
{
    public class Healthbar : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private Image _foreground;
        #endregion

        #region FIELDS PRIVATE
        private HealthComponent _health;
        #endregion

        #region HANDLERS
        private void Health_OnChangeHP(uint maxHP, int currentHP)
        {
            UpdateHealthbar(maxHP, currentHP);
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnDestroy()
        {
            _health.OnChangeHP -= Health_OnChangeHP;
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(HealthComponent health)
        {
            _health = health;
            _health.OnChangeHP += Health_OnChangeHP;
        }

        public void UpdateHealthbar(float maxHP, float currentHP)
        {
            _foreground.fillAmount = currentHP / maxHP;
        }
        #endregion
    }
}
