using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using BossCortege.EventHolder;

namespace BossCortege
{
    [RequireComponent(typeof(GuardCar))]
    public class MergeComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler
    {
        #region FIELDS PRIVATE
        private const float AcceptHalfX = 0.95f;
        private const float AcceptHalfZ = 1.22f;
        private const float CarScoreBias = 0.88f;
        private const float FlyHeight = 0.8f;
        private const float PickupScale = 1.07f;
        private const float HoverScale = 1.12f;
        private const float ReturnDuration = 0.16f;
        private const float CoveragePadding = 1.18f;
        private const float PickupPadding = 1.02f;
        private const float PickupPixelPad = 14f;
        private const float AimAssistDistance = 10f;

        private static MergeComponent _pointerReceiver;
        private static MergeComponent _draggedCar;

        private Camera _camera;

        private GuardCar _car;
        private PlaceComponent _place;
        private BoxCollider _collider;

        private Vector3 _bodyScale;
        private bool _isHovered;

        private bool _isDragging;
        private bool _aimAssist;
        private int _pointerId;
        private Vector2 _pointerPosition;
        private Vector2 _pressPosition;
        private float _groundY;
        private Vector3 _aimPoint;

        private MergeComponent[] _cars;
        private AbstractPlace[] _places;
        private MergeComponent _hovered;
        private Renderer[] _bodyRenderers;
        private DropTarget _dropTarget;
        #endregion

        #region PROPERTIES
        public GuardCar Car => _car;
        public PlaceComponent Place => _place;
        #endregion

        #region UNITY CALLBACKS
        private void OnEnable()
        {
            EnlargeCollider();
        }

        private void OnDisable()
        {
            transform.DOKill();
            ClearHover();
            RestoreBodyScale();

            if (_pointerReceiver == this)
            {
                var dragged = _draggedCar;
                _pointerReceiver = null;
                _draggedCar = null;
                if (dragged != null && dragged != this) dragged.CancelDrag();
            }

            if (!_isDragging) return;

            _isDragging = false;
            if (_draggedCar == this) _draggedCar = null;
            EventHolder<DragCarInfo>.NotifyListeners(new DragCarInfo(false));
        }

        private void Update()
        {
            if (!_isDragging) return;

            ReadPointer();
            ApplyDragPosition();
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_camera == null) _camera = Camera.main;
            if (_draggedCar != null) return;

            var tapped = FindTappedCar(eventData.position);
            if (tapped == null || tapped._place == null || tapped._place.Place == null) return;

            _pointerReceiver = this;
            _draggedCar = tapped;
            tapped.BeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_pointerReceiver != this || _draggedCar == null) return;

            _draggedCar.SetPointer(eventData.position);
            _draggedCar.ApplyDragPosition();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerReceiver != this) return;

            var dragged = _draggedCar;
            _pointerReceiver = null;
            _draggedCar = null;
            if (dragged != null) dragged.EndDrag(eventData);
        }
        #endregion

        #region METHODS PRIVATE
        private void ReadPointer()
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);
                    if (touch.fingerId != _pointerId) continue;

                    _pointerPosition = touch.position;
                    return;
                }

                return;
            }

            _pointerPosition = Input.mousePosition;
        }

        private void BeginDrag(PointerEventData eventData)
        {
            if (_camera == null) _camera = Camera.main;
            if (_place == null || _place.Place == null) return;

            transform.DOKill();

            _isDragging = true;
            _aimAssist = false;
            _pointerId = eventData.pointerId;
            _pointerPosition = eventData.position;
            _pressPosition = eventData.position;
            _groundY = _place.Place.SpawnPoint.position.y;
            _dropTarget = default;

            _cars = FindObjectsByType<MergeComponent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            _places = FindObjectsByType<AbstractPlace>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            if (_car.Body != null)
            {
                _car.Body.localScale = _bodyScale * PickupScale;
            }

            EventHolder<DragCarInfo>.NotifyListeners(new DragCarInfo(true));
            ApplyDragPosition();
        }

        private void SetPointer(Vector2 screen)
        {
            _pointerPosition = screen;
        }

        private void EndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            _pointerPosition = eventData.position;
            _isDragging = false;
            if (_draggedCar == this) _draggedCar = null;
            EventHolder<DragCarInfo>.NotifyListeners(new DragCarInfo(false));

            ApplyDragPosition();
            TryDrop();
        }

        private void CancelDrag()
        {
            if (!_isDragging) return;

            _isDragging = false;
            _dropTarget = default;
            ClearHover();
            RestoreBodyScale();
            EventHolder<DragCarInfo>.NotifyListeners(new DragCarInfo(false));
            ReturnHome();
        }

        private MergeComponent FindTappedCar(Vector2 screen)
        {
            if (_camera == null) _camera = Camera.main;

            var cars = FindObjectsByType<MergeComponent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            MergeComponent best = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < cars.Length; i++)
            {
                var car = cars[i];
                if (car == null || car._place == null || car._place.Place == null) continue;
                if (!car.TryGetPickupBounds(out ScreenCoverage bounds)) continue;
                if (!bounds.Contains(screen)) continue;

                float distance = (bounds.Center - screen).sqrMagnitude;
                if (distance >= bestDistance) continue;

                bestDistance = distance;
                best = car;
            }

            return best;
        }

        private bool TryGetPickupBounds(out ScreenCoverage bounds)
        {
            bounds = GetCoverage(PickupPadding);
            if (!bounds.Valid) return false;

            bounds.Half += new Vector2(PickupPixelPad, PickupPixelPad);
            return true;
        }

        private void ApplyDragPosition()
        {
            if (!_aimAssist && (_pointerPosition - _pressPosition).sqrMagnitude >= AimAssistDistance * AimAssistDistance)
            {
                _aimAssist = true;
            }

            if (!TryProjectPointer(out Vector3 pointerGround))
            {
                _dropTarget = default;
                return;
            }

            var lifted = pointerGround;
            lifted.y = _groundY + FlyHeight;
            transform.position = lifted;

            if (!_aimAssist)
            {
                ClearHover();
                _dropTarget = default;
                _aimPoint = pointerGround;
                return;
            }

            var coverage = GetCoverage();
            var perceived = coverage.Valid ? ScreenPointOnGround(coverage.Center, pointerGround) : pointerGround;
            var target = FindDropTarget(perceived, coverage);

            if (target.Found)
            {
                float closeness = Mathf.Clamp01(1f - target.Score);
                float pull = closeness * closeness * 0.8f;
                var delta = target.Position - perceived;
                delta.y = 0f;
                transform.position += delta * pull;

                coverage = GetCoverage();
                perceived = coverage.Valid ? ScreenPointOnGround(coverage.Center, perceived + delta * pull) : perceived + delta * pull;
                var settled = FindDropTarget(perceived, coverage);
                if (settled.Found) target = settled;
            }

            UpdateHover(target);
            _aimPoint = perceived;
            _dropTarget = target;
        }

        private bool TryProjectPointer(out Vector3 point)
        {
            point = transform.position;
            if (_camera == null) return false;

            return TryPlanePoint(_camera.ScreenPointToRay(_pointerPosition), out point);
        }

        private Vector3 ScreenPointOnGround(Vector2 screen, Vector3 fallback)
        {
            if (_camera == null) return fallback;
            return TryPlanePoint(_camera.ScreenPointToRay(screen), out Vector3 point) ? point : fallback;
        }

        private bool TryPlanePoint(Ray ray, out Vector3 point)
        {
            var plane = new Plane(Vector3.up, new Vector3(0f, _groundY, 0f));
            if (!plane.Raycast(ray, out float distance))
            {
                point = transform.position;
                return false;
            }

            point = ray.GetPoint(distance);
            return true;
        }

        private ScreenCoverage GetCoverage()
        {
            return GetCoverage(CoveragePadding);
        }

        private ScreenCoverage GetCoverage(float padding)
        {
            var coverage = new ScreenCoverage();
            if (_bodyRenderers == null) CacheBodyRenderers();
            if (_camera == null || _bodyRenderers == null || _bodyRenderers.Length == 0) return coverage;

            bool any = false;
            Bounds bounds = default;
            for (int i = 0; i < _bodyRenderers.Length; i++)
            {
                var renderer = _bodyRenderers[i];
                if (renderer == null || !renderer.enabled) continue;
                if (!any)
                {
                    bounds = renderer.bounds;
                    any = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            if (!any) return coverage;

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            var center = bounds.center;
            var extents = bounds.extents;
            for (int i = 0; i < 8; i++)
            {
                var corner = center + new Vector3(
                    (i & 1) == 0 ? -extents.x : extents.x,
                    (i & 2) == 0 ? -extents.y : extents.y,
                    (i & 4) == 0 ? -extents.z : extents.z);
                var screen = _camera.WorldToScreenPoint(corner);
                if (screen.z <= 0f) continue;

                var flat = new Vector2(screen.x, screen.y);
                min = Vector2.Min(min, flat);
                max = Vector2.Max(max, flat);
            }

            if (min.x >= max.x || min.y >= max.y) return coverage;

            coverage.Valid = true;
            coverage.Center = (min + max) * 0.5f;
            coverage.Half = (max - min) * (0.5f * padding);
            return coverage;
        }

        private void TryDrop()
        {
            var target = _dropTarget.Found ? _dropTarget : FindDropTarget(_aimPoint, GetCoverage());
            ClearHover();
            RestoreBodyScale();
            if (target.Car != null)
            {
                if (target.Car.Car.Config.Level == _car.Config.Level)
                {
                    EventHolder<MergeCarInfo>.NotifyListeners(new MergeCarInfo(_car, _place, target.Car.Car, target.Car.Place));
                }
                else
                {
                    EventHolder<SwapCarInfo>.NotifyListeners(new SwapCarInfo(_place, target.Car.Place));
                }

                return;
            }

            if (target.Place != null && target.Place.IsVacant)
            {
                _place.Replace();
                target.Place.PlaceVechicle(_place);
                EventHolder<ReplaceCarInfo>.NotifyListeners(null);
                return;
            }

            ReturnHome();
        }

        private DropTarget FindDropTarget(Vector3 probe, ScreenCoverage coverage)
        {
            var best = new DropTarget { Score = float.MaxValue };

            if (_cars != null)
            {
                for (int i = 0; i < _cars.Length; i++)
                {
                    var other = _cars[i];
                    if (other == null || other == this || other.Car == null) continue;

                    var position = other.transform.position;
                    float score = TargetScore(probe, coverage, position, VisualPoint(other)) * CarScoreBias;
                    if (score >= best.Score) continue;

                    best.Score = score;
                    best.Car = other;
                    best.Place = null;
                    best.Position = position;
                }
            }

            if (_places != null)
            {
                for (int i = 0; i < _places.Length; i++)
                {
                    var place = _places[i];
                    if (place == null || place.SpawnPoint == null || !place.IsVacant) continue;

                    var position = place.SpawnPoint.position;
                    float score = TargetScore(probe, coverage, position, position);
                    if (score >= best.Score) continue;

                    best.Score = score;
                    best.Car = null;
                    best.Place = place;
                    best.Position = position;
                }
            }

            best.Found = best.Score <= 1f;
            if (!best.Found)
            {
                best.Car = null;
                best.Place = null;
            }

            return best;
        }

        private float TargetScore(Vector3 probe, ScreenCoverage coverage, Vector3 groundPoint, Vector3 visualPoint)
        {
            float ground = SlotScore(probe, groundPoint);
            if (!coverage.Valid || _camera == null) return ground;

            var screen = _camera.WorldToScreenPoint(visualPoint);
            if (screen.z <= 0f) return ground;

            float nx = (screen.x - coverage.Center.x) / coverage.Half.x;
            float ny = (screen.y - coverage.Center.y) / coverage.Half.y;
            float visible = nx * nx + ny * ny;
            return Mathf.Min(ground, visible);
        }

        private static float SlotScore(Vector3 probe, Vector3 target)
        {
            float dx = (probe.x - target.x) / AcceptHalfX;
            float dz = (probe.z - target.z) / AcceptHalfZ;
            return dx * dx + dz * dz;
        }

        private static Vector3 VisualPoint(MergeComponent car)
        {
            if (car != null && car.Car != null && car.Car.Body != null)
            {
                var renderers = car.Car.Body.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] == null || renderers[i] is ParticleSystemRenderer) continue;
                    return renderers[i].bounds.center;
                }
            }

            return car.transform.position;
        }

        private void UpdateHover(DropTarget target)
        {
            var next = target.Found ? target.Car : null;
            if (_hovered == next) return;

            if (_hovered != null) _hovered.SetHovered(false);
            _hovered = next;
            if (_hovered != null) _hovered.SetHovered(true);
        }

        private void ClearHover()
        {
            if (_hovered != null) _hovered.SetHovered(false);
            _hovered = null;
        }

        private void RestoreBodyScale()
        {
            if (_car != null && _car.Body != null)
            {
                _car.Body.localScale = _bodyScale;
            }
        }

        private void ReturnHome()
        {
            if (_place == null || _place.Place == null) return;

            transform.DOKill();
            transform.DOMove(_place.Place.SpawnPoint.position, ReturnDuration).SetEase(Ease.OutQuad);
        }

        public void SetHovered(bool hovered)
        {
            if (_isHovered == hovered) return;
            _isHovered = hovered;

            if (_car == null || _car.Body == null) return;
            _car.Body.localScale = hovered ? _bodyScale * HoverScale : _bodyScale;
        }
        #endregion

        #region METHODS PUBLIC
        public void Init(GuardCar car, PlaceComponent place)
        {
            _car = car;
            _place = place;
            _camera = Camera.main;
            _collider = GetComponent<BoxCollider>();
            _bodyScale = _car.Body != null ? _car.Body.localScale : Vector3.one;
            CacheBodyRenderers();

            EnlargeCollider();
        }

        private void CacheBodyRenderers()
        {
            if (_car == null || _car.Body == null)
            {
                _bodyRenderers = System.Array.Empty<Renderer>();
                return;
            }

            var all = _car.Body.GetComponentsInChildren<Renderer>(true);
            int count = 0;
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] is ParticleSystemRenderer) continue;
                count++;
            }

            _bodyRenderers = new Renderer[count];
            int index = 0;
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] is ParticleSystemRenderer) continue;
                _bodyRenderers[index++] = all[i];
            }
        }

        private void EnlargeCollider()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider>();
            if (_collider == null) return;

            _collider.enabled = true;
            _collider.size = new Vector3(1.6f, 1.35f, 2.1f);
            _collider.center = new Vector3(0f, 0.6f, 0f);
        }
        #endregion

        private struct DropTarget
        {
            public bool Found;
            public MergeComponent Car;
            public AbstractPlace Place;
            public Vector3 Position;
            public float Score;
        }

        private struct ScreenCoverage
        {
            public bool Valid;
            public Vector2 Center;
            public Vector2 Half;

            public bool Contains(Vector2 screen)
            {
                if (!Valid || Half.x <= 0f || Half.y <= 0f) return false;

                float nx = (screen.x - Center.x) / Half.x;
                float ny = (screen.y - Center.y) / Half.y;
                return nx * nx + ny * ny <= 1f;
            }
        }
    }
}
