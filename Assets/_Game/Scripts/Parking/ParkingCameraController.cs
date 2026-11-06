using UnityEngine;
using BossCortege.EventHolder;


namespace BossCortege
{
    public class ParkingCameraController : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private float _mouseSensitivity = 0.02f;
        private Vector2 _tapPosition;
        private bool _isMobile;
        private bool _isDragging = false;

        private float _cameraSpeed = 15f;

        private Vector3 _cameraPosition;
        #endregion

        #region HANDLERS
        private void DragCarHandler(DragCarInfo info)
        {
            _tapPosition = Vector2.zero;
            _isDragging = info.IsDragging;
        }
        #endregion

        #region UNITY CALLBACKS
        private void Awake()
        {
            _isMobile = Application.isMobilePlatform;
            _cameraPosition = transform.position;
        }

        private void OnEnable()
        {
            EventHolder<DragCarInfo>.AddListener(DragCarHandler, false);
        }

        private void OnDisable()
        {
            EventHolder<DragCarInfo>.RemoveListener(DragCarHandler);
        }

        private void Update()
        {
            if (_isDragging) return;

            if (PointerInput.TryGetPrimary(_isMobile, out var pointer) && pointer.Began)
            {
                _tapPosition = pointer.Position;
            }

            MoveCamera();

            transform.position = Vector3.Lerp(transform.position, _cameraPosition, _cameraSpeed * Time.deltaTime);
        }
        #endregion

        #region METHODS PRIVATE
        private void GetTapPosition()
        {
            if (!PointerInput.TryGetPrimary(_isMobile, out var pointer)) return;

            _tapPosition = pointer.Position;
        }

        private void MoveCamera()
        {
            if (_tapPosition == Vector2.zero) return;

            var swipeDelta = Vector2.zero;
            if (PointerInput.TryGetPrimary(_isMobile, out var pointer) && pointer.Held)
            {
                swipeDelta = pointer.Position - _tapPosition;
            }

            var positionZ = Mathf.Clamp(transform.position.z + (-swipeDelta.y * _mouseSensitivity), -10f, 0);
            //var cameraPosition = new Vector3(transform.position.x, transform.position.y, positionZ);
            //transform.position = cameraPosition;

            _cameraPosition = new Vector3(transform.position.x, transform.position.y, positionZ);

            GetTapPosition();
        }
        #endregion
    }
}
