using UnityEngine;
using UnityEngine.EventSystems;
using BossCortege.EventHolder;

namespace BossCortege
{
    [RequireComponent(typeof(GuardCar))]
    public class MergeComponent : MonoBehaviour, IBeginDragHandler, IDragHandler , IEndDragHandler
    {
        #region FIELDS PRIVATE
        private Camera _camera;

        private GuardCar _car;
        private PlaceComponent _place;

        private BoxCollider _collider;
        #endregion

        #region PROPERTIES
        public GuardCar Car => _car;
        public PlaceComponent Place => _place;
        #endregion

        #region UNITY CALLBACKS
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

                var merger = hit.collider.GetComponentInParent<MergeComponent>();
                if (merger != null)
                {
                    if(merger.Car.Config.Level == _car.Config.Level)
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