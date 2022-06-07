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
            CheckDamage(maxHP, currentHP);
        }
        #endregion

        #region METHODS PUBLIC
        public void CheckDamage(float maxHP, float currentHP)
        {
            if (_smoke.isPlaying) return;

            if((currentHP / maxHP) <= (_percent / 100))
            {
                _smoke.Play();
            }
        }
        #endregion
    }
}
