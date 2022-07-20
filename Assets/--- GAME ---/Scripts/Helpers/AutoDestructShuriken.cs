using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestructShuriken : MonoBehaviour
{
    #region FIELDS INSPECTOR
    [SerializeField] private bool _onlyDeactivate;
    #endregion

    #region FIELDS PRIVATE
    private ParticleSystem _particleSystem;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckIfAlive());
    }
    #endregion

    #region COROUTINES
    IEnumerator CheckIfAlive()
    {
        while (true && _particleSystem != null)
        {
            yield return new WaitForSeconds(0.5f);

            if (!_particleSystem.IsAlive(true))
            {
                if (_onlyDeactivate)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    Destroy(gameObject);
                }

                break;
            }
        }
    }
    #endregion
}
