using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace BossCortege
{
    public readonly struct PointerSample
    {
        public readonly bool Began;
        public readonly bool Ended;
        public readonly bool Pressed;
        public readonly bool Held;
        public readonly Vector2 Position;

        public PointerSample(bool began, bool ended, bool pressed, bool held, Vector2 position)
        {
            Began = began;
            Ended = ended;
            Pressed = pressed;
            Held = held;
            Position = position;
        }
    }

    public static class PointerInput
    {
        private struct CachedTouch
        {
            public int Id;
            public Vector2 Position;
        }

        private static PointerSample _sample;
        private static bool _hasDevice;
        private static bool _wasHeld;
        private static bool _latchedBegan;
        private static bool _latchedEnded;
        private static bool _latchedTouchEnd;
        private static bool _installed;
        private static int _capturedFrame = -1;
        private static CachedTouch[] _touches = new CachedTouch[8];
        private static int _touchCount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAndInstall()
        {
            _sample = default;
            _hasDevice = false;
            _wasHeld = false;
            _latchedBegan = false;
            _latchedEnded = false;
            _latchedTouchEnd = false;
            _capturedFrame = -1;
            _touchCount = 0;
            Install();
        }

        public static bool TryGetPrimary(bool isMobile, out PointerSample sample)
        {
            Install();
            sample = _sample;
            return _hasDevice;
        }

        public static bool TryReadTracked(int pointerId, out Vector2 position)
        {
            Install();

            for (var i = 0; i < _touchCount; i++)
            {
                if (_touches[i].Id != pointerId) continue;

                position = _touches[i].Position;
                return true;
            }

            if (_touchCount > 0)
            {
                position = default;
                return false;
            }

            if (!_hasDevice)
            {
                position = default;
                return false;
            }

            position = _sample.Position;
            return true;
        }

        private static void Install()
        {
            if (_installed) return;

            InputSystem.onAfterUpdate -= Capture;
            InputSystem.onAfterUpdate += Capture;
            _installed = true;
        }

        private static void Capture()
        {
            if (!IsGameplayUpdate()) return;

            CacheTouches();

            var pointer = Pointer.current;
            if (pointer == null)
            {
                _hasDevice = false;
                _wasHeld = false;
                return;
            }

            var frame = Time.frameCount;
            if (frame != _capturedFrame)
            {
                _capturedFrame = frame;
                _latchedBegan = false;
                _latchedEnded = false;
                _latchedTouchEnd = false;
            }

            var held = pointer.press.ReadValue() > 0f;
            var began = held && !_wasHeld;
            var ended = !held && _wasHeld;
            _wasHeld = held;
            if (began) _latchedBegan = true;
            if (ended) _latchedEnded = true;
            if (ended && pointer is Touchscreen) _latchedTouchEnd = true;

            _hasDevice = true;
            _sample = new PointerSample(_latchedBegan, _latchedEnded, held, held || _latchedTouchEnd, pointer.position.ReadValue());
        }

        private static bool IsGameplayUpdate()
        {
            var update = InputState.currentUpdateType;
            var mode = InputSystem.settings.updateMode;
            if (mode == InputSettings.UpdateMode.ProcessEventsInFixedUpdate)
                return update == InputUpdateType.Fixed;

            return update == InputUpdateType.Dynamic || update == InputUpdateType.Manual;
        }

        private static void CacheTouches()
        {
            _touchCount = 0;
            var touchscreen = Touchscreen.current;
            if (touchscreen == null) return;

            var touches = touchscreen.touches;
            for (var i = 0; i < touches.Count; i++)
            {
                var touch = touches[i];
                var id = touch.touchId.ReadValue();
                var pressed = touch.press.ReadValue() > 0f;
                if (id == 0 && !pressed) continue;
                if (_touchCount >= _touches.Length)
                    System.Array.Resize(ref _touches, _touches.Length * 2);

                _touches[_touchCount++] = new CachedTouch
                {
                    Id = id,
                    Position = touch.position.ReadValue()
                };
            }
        }
    }
}
