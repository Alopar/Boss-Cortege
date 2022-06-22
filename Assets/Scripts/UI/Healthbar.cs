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
        private IDamageable _damageable;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _damageable = GetComponentInParent<IDamageable>();
        }

        private void OnEnable()
        {
            if (_damageable == null) return;
            _damageable.OnDamage += Damageable_OnDamage;
        }

        private void OnDisable()
        {
            if (_damageable == null) return;
            _damageable.OnDamage -= Damageable_OnDamage;
        }
        #endregion

        #region HANDLERS
        private void Damageable_OnDamage(uint maxHP, int currentHP)
        {
            UpdateHealthbar(maxHP, currentHP);
        }
        #endregion

        #region METHODS PUBLIC
        public void UpdateHealthbar(float maxHP, float currentHP)
        {
            _foreground.fillAmount = currentHP / maxHP;
        }
        #endregion
    }
}
