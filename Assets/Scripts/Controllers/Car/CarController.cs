using UnityEngine;
using UnityEngine.EventSystems;

namespace BossCortege
{
    [SelectionBase]
    public class CarController : MonoBehaviour, IBeginDragHandler, IDragHandler , IEndDragHandler
    {
        #region FIELDS INSPECTOR
        [SerializeField] private bool _isLimo;
        #endregion

        #region FIELDS PRIVATE
        private Car _settings;
        private Place _place;

        private Collider _collider;
        private Rigidbody _rigidbody;

        private Camera _camera;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Only the initial installation is available
        /// </summary>
        public Car Settings
        {
            get { return _settings; }
            set
            {
                if(_settings == null)
                {
                    _settings = value;
                }
            }
        }
        public Place Place { get { return _place; } set { _place = value; } }
        public bool IsLimo => _isLimo;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _collider = GetComponentInChildren<BoxCollider>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isLimo) return;

            var hoverPosition = transform.position;
            hoverPosition.y += 1;
            transform.position = hoverPosition;

            _collider.enabled = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isLimo) return;

            var mouseZ = _camera.WorldToScreenPoint(transform.position).z;
            var screenPosition = new Vector3(eventData.position.x, eventData.position.y, mouseZ);
            var worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            transform.position = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isLimo) return;

            var ray = _camera.ScreenPointToRay(eventData.position);
            if(Physics.Raycast(ray, out RaycastHit hit, 99f))
            {
                _collider.enabled = true;

                var car = hit.collider.GetComponentInParent<CarController>();
                if (car != null && !car.IsLimo)
                {
                    if(car.Settings.Level == _settings.Level)
                    {
                        GameManager.Instance.MergeCar(this, car);
                    }
                    else
                    {
                        GameManager.Instance.SwapCar(this, car);
                    }

                    return;
                }

                var place = hit.collider.GetComponent<Place>();
                if (place != null && place != _place && place.Car == null)
                {
                    _place.ClearPlace();
                    place.PlaceCar(this);
                    return;
                }
            }

            _place.PlaceCar(this);
        }
        #endregion
    }
}