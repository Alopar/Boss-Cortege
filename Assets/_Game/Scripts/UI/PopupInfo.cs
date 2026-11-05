using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace BossCortege
{
    public class PopupInfo : MonoBehaviour
    {
        #region FIELDS INSPECTOR
        [SerializeField] private float _lifeTime;
        [SerializeField] private float _popupDistance;

        [Space(10)]
        [SerializeField] private CanvasGroup _body;
        [SerializeField] private TextMeshProUGUI _text;
        #endregion

        #region METHODS PUBLIC
        public void SetText(string text)
        {
            _text.text = text;

            transform.DOMoveY(transform.position.y + _popupDistance, _lifeTime).OnComplete(() => Destroy(gameObject)).SetEase(Ease.OutBack);
            DOVirtual.Float(_body.alpha, 0.2f, _lifeTime, (v) => { _body.alpha = v; }).SetEase(Ease.InCubic);
        }
        #endregion
    }
}
