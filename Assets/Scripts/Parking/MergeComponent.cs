using UnityEngine;
using UnityEngine.EventSystems;
using BossCortege.EventHolder;

namespace BossCortege
{
    [RequireComponent(typeof(GuardCar))]
    public class MergeComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        #region FIELDS PRIVATE
        private Camera _camera;

        private GuardCar _car;
        private PlaceComponent _place;

        private BoxCollider _collider;

        private float _flySpeed = 25f;

        private float _flyHeight = 0.5f;
        private float _currentflyHeight;

        private bool _isDragging = false;
        private Vector3 _currentPosition;
        #endregion

        #region PROPERTIES
        public GuardCar Car => _car;
        public PlaceComponent Place => _place;
        #endregion

        #region UNITY CALLBACKS
        private void Update()
        {
            if (_isDragging)
            {
                transform.position = Vector3.Lerp(transform.position, _currentPosition, _flySpeed * Time.deltaTime);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            _currentflyHeight = transform.position.y + _flyHeight;
            _collider.enabled = false;

            var worldPosition = transform.position;
            worldPosition.y = _currentflyHeight;

            _currentPosition = worldPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
            CheckMerge(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var mouseZ = _camera.WorldToScreenPoint(transform.position).z;
            var screenPosition = new Vector3(eventData.position.x, eventData.position.y, mouseZ);
            var worldPosition = _camera.ScreenToWorldPoint(screenPosition);

            worldPosition.y = _currentflyHeight;
            worldPosition.z += 0.5f;

            _currentPosition = worldPosition;
        }
        #endregion

        #region METHODS PRIVATE
        private void CheckMerge(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit, 99f))
            {
                _collider.enabled = true;

                var merger = hit.collider.GetComponentInParent<MergeComponent>();
                if (merger != null)
                {
                    if (merger.Car.Config.Level == _car.Config.Level)
                    {
                        EventHolder<MergeCarInfo>.NotifyListeners(new MergeCarInfo(_car, _place, merger.Car, merger.Place));
                    }
                    else
                    {
                        EventHolder<SwapCarInfo>.NotifyListeners(new SwapCarInfo(_place, merger.Place));
                    }

                    return;
                }

                var place = hit.collider.GetComponent<AbstractPlace>();
                if (place != null && place != _place && place.IsVacant)
                {
                    _place.Replace();
                    place.PlaceVechicle(_place);
                    return;
                }
            }

            _place.ReturnToPlace();
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(GuardCar car, PlaceComponent place)
        {
            _car = car;
            _place = place;

            _camera = Camera.main;
            _collider = GetComponentInChildren<BoxCollider>();
        }
        #endregion

    }
}