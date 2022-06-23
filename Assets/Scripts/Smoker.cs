using UnityEngine;

namespace BossCortege
{
    public class Smoker : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private ParticleSystem _smoke;
        [SerializeField, Range(0, 100), Tooltip("На каком количестве процентов здоровья начинается пожар")] private float _percent;
        #endregion

        #region FIELDS PRIVATE
        private HealthComponent _health;
        #endregion

        #region HANDLERS
        private void Health_OnChangeHP(uint maxHP, int currentHP)
        {
            CheckDamage(maxHP, currentHP);
        }
        #endregion

        #region UNITY CALLBACKS
        private void OnDestroy()
        {
            _health.OnChangeHP -= Health_OnChangeHP;
        }
        #endregion

        #region METHODS PRIVATE
        private void CheckDamage(float maxHP, float currentHP)
        {
            if (_smoke.isPlaying) return;

            if ((currentHP / maxHP) <= (_percent / 100))
            {
                _smoke.Play();
            }
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(HealthComponent health)
        {
            _health = health;
            _health.OnChangeHP += Health_OnChangeHP;
        }
        #endregion

    }
}
