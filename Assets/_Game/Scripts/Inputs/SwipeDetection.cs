using UnityEngine;
using BossCortege.EventHolder;

namespace BossCortege
{
    public class SwipeDetection : MonoBehaviour
    {
        #region FIELDS PRIVATE
        private Vector2 tapPosition;
        private Vector2 swipeDelta;

        private float deadZone = 80;

        private bool isSwiping;
        private bool isMobile;
        private bool wasPressed;
        #endregion

        #region UNITY CALLBACKS
        private void Start()
        {
            isMobile = Application.isMobilePlatform;
        }

        private void Update()
        {
            if (!PointerInput.TryGetPrimary(isMobile, out var pointer))
            {
                if (wasPressed) ResetSwipe();
                wasPressed = false;
                return;
            }

            if (pointer.Pressed && !wasPressed)
            {
                isSwiping = true;
                tapPosition = pointer.Position;
            }
            else if (!pointer.Pressed && wasPressed)
            {
                ResetSwipe();
            }

            wasPressed = pointer.Pressed;
            CheckSwipe();
        }
        #endregion

        #region METHODS PRIVATE
        private void CheckSwipe()
        {
            swipeDelta = Vector2.zero;

            if (isSwiping && PointerInput.TryGetPrimary(isMobile, out var pointer) && pointer.Pressed)
            {
                swipeDelta = pointer.Position - tapPosition;
            }

            if (swipeDelta.magnitude > deadZone)
            {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    EventHolder<InputSwipeInfo>.NotifyListeners(new InputSwipeInfo(swipeDelta.x > 0 ? Vector2.right : Vector2.left));
                }
                else
                {
                    EventHolder<InputSwipeInfo>.NotifyListeners(new InputSwipeInfo(swipeDelta.y > 0 ? Vector2.up : Vector2.down));
                }

                ResetSwipe();
            }
        }

        private void ResetSwipe()
        {
            isSwiping = false;
            tapPosition = Vector2.zero;
            swipeDelta = Vector2.zero;
        }
        #endregion
    }
}
