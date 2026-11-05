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

            if (!_isMobile)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GetTapPosition();
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        GetTapPosition();
                    }
                }
            }

            MoveCamera();

            transform.position = Vector3.Lerp(transform.position, _cameraPosition, _cameraSpeed * Time.deltaTime);
        }
        #endregion

        #region METHODS PRIVATE
        private void GetTapPosition()
        {
            if (!_isMobile)
            {
                _tapPosition = Input.mousePosition;
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    _tapPosition = Input.GetTouch(0).position;
                }
            }
        }

        private void MoveCamera()
        {
            if (_tapPosition == Vector2.zero) return;

            var swipeDelta = Vector2.zero;
            if (!_isMobile && Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - _tapPosition;
            }
            else if (Input.touchCount > 0)
            {
                swipeDelta = Input.GetTouch(0).position - _tapPosition;
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
