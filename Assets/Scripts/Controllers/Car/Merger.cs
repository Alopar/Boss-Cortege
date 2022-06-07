using UnityEngine;
using UnityEngine.EventSystems;

namespace BossCortege
{
    [RequireComponent(typeof(GuardParkingController))]
    public class Merger : MonoBehaviour, IBeginDragHandler, IDragHandler , IEndDragHandler
    {
        #region FIELDS PRIVATE
        private Camera _camera;
        private BoxCollider _collider;
        private GuardParkingController _parkingController;
        #endregion

        #region PROPERTIES
        public GuardParkingController Parking => _parkingController;
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _collider = GetComponentInChildren<BoxCollider>();
            _parkingController = GetComponent<GuardParkingController>();
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

                var car = hit.collider.GetComponentInParent<Merger>();
                if (car != null)
                {
                    if(car.Parking.Config.Level == _parkingController.Config.Level)
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
                if (place != null && place != _parkingController.Place && place.IsVacant)
                {
                    _parkingController.Replace();
                    place.TryPlaceVechicle(_parkingController);
                    return;                    
                }
            }

            _parkingController.ReturnToPlace();
        }
        #endregion
    }
}