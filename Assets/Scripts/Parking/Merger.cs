using UnityEngine;
using UnityEngine.EventSystems;
using BossCortege.EventHolder;

namespace BossCortege
{
    [RequireComponent(typeof(GuardCar))]
    public class Merger : MonoBehaviour, IBeginDragHandler, IDragHandler , IEndDragHandler
    {
        #region FIELDS PRIVATE
        private Camera _camera;
        private GuardCar _car;
        private BoxCollider _collider;
        #endregion

        #region PROPERTIES
        public GuardCar Car => _car;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _car = GetComponent<GuardCar>();
            _collider = GetComponentInChildren<BoxCollider>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var hoverPosition = transform.position;
            hoverPosition.y += 1;
            transform.position = hoverPosition;

            _collider.enabled = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var mouseZ = _camera.WorldToScreenPoint(transform.position).z;
            var screenPosition = new Vector3(eventData.position.x, eventData.position.y, mouseZ);
            var worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            transform.position = new Vector3(worldPosition.x, transform.position.y, worldPosition.z + 0.5f);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(eventData.position);
            if(Physics.Raycast(ray, out RaycastHit hit, 99f))
            {
                _collider.enabled = true;

                var merger = hit.collider.GetComponentInParent<Merger>();
                if (merger != null)
                {
                    if(merger.Car.Config.Level == _car.Config.Level)
                    {
                        EventHolder<MergeCarInfo>.NotifyListeners(new MergeCarInfo(_car, merger.Car));
                    }
                    else
                    {
                        EventHolder<SwapCarInfo>.NotifyListeners(new SwapCarInfo(_car, merger.Car));
                    }

                    return;
                }

                var place = hit.collider.GetComponent<Place>();
                if (place != null && place != _car.Place && place.IsVacant)
                {
                    _car.Replace();
                    place.TryPlaceVechicle(_car);
                    return;                    
                }
            }

            _car.ReturnToPlace();
        }
        #endregion
    }
}