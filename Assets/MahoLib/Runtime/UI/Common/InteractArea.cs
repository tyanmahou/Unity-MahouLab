using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace Maho.UI.Common
{
    [DefaultExecutionOrder(-1001)]
    [RequireComponent(typeof(CanvasRenderer))]
    public class InteractArea : UIBehaviour
    {
        [SerializeField] private bool _interactable = true;
        [SerializeField] private UnityEvent _onDown = new UnityEvent();
        [SerializeField] private UnityEvent _onClick = new UnityEvent();
        public bool interactable
        {
            get => _interactable;
            set => _interactable = value;
        }
        public UnityEvent onDown
        {
            get => _onDown;
            set => _onDown = value;
        }
        public UnityEvent onClick
        {
            get => _onClick;
            set => _onClick = value;
        }
        public Canvas canvas
        {
            get
            {
                if (_canvas == null)
                    CacheCanvas();
                return _canvas;
            }
        }
        public CanvasRenderer canvasRenderer
        {
            get
            {
                if (ReferenceEquals(_canvasRenderer, null))
                {
                    _canvasRenderer = GetComponent<CanvasRenderer>();

                    if (ReferenceEquals(_canvasRenderer, null))
                    {
                        _canvasRenderer = gameObject.AddComponent<CanvasRenderer>();
                    }
                }
                return _canvasRenderer;
            }
        }
        public bool IsInteractable()
        {
            return _groupsAllowInteraction && _interactable;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _groupsAllowInteraction = ParentGroupAllowsInteraction();
            CacheCanvas();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            _isPointerDown = false;
        }
        protected override void OnCanvasHierarchyChanged()
        {
            _canvas = null;
            if (!IsActive())
            {
                return;
            }
            CacheCanvas();
        }
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            _canvas = null;
            if (!IsActive())
            {
                return;
            }
            CacheCanvas();
        }
        private void Update()
        {
            if (!IsInteractable())
            {
                _isPointerDown = false;
                return;
            }
            EventSystem eventSystem = EventSystem.current;
            if (IsPointerDown(eventSystem))
            {
                _isPointerDown = true;
                _onDown?.Invoke();
            }
            if (IsPointerClick(eventSystem))
            {
                onClick?.Invoke();
            }
        }
        private bool IsPointerDown(EventSystem eventSystem)
            => IsPointerDown(eventSystem, out Vector2 mousePosition) && IsPassThroughRaycast(eventSystem, mousePosition);
        private bool IsPointerDown(EventSystem eventSystem, out Vector2 mousePosition)
        {
            mousePosition = Vector2.zero;
            if (eventSystem == null)
            {
                return false;
            }
            var inputModule = eventSystem.currentInputModule;
            if (inputModule == null)
            {
                return false;
            }
            var input = inputModule.input;
            if (input == null)
            {
                mousePosition = Input.mousePosition;
                return Input.GetMouseButtonDown(0);
            }
            else
            {
                mousePosition = input.mousePosition;
                return input.GetMouseButtonDown(0);
            }
        }
        private bool IsPointerClick(EventSystem eventSystem)
        {
            if (!_isPointerDown)
            {
                return false;
            }
            if (!IsPointerUp(eventSystem, out Vector2 mousePosition))
            {
                return false;
            }
            _isPointerDown = false;
            return IsPassThroughRaycast(eventSystem, mousePosition);

        }
        private bool IsPointerUp(EventSystem eventSystem, out Vector2 mousePosition)
        {
            mousePosition = Vector2.zero;
            if (eventSystem == null)
            {
                return false;
            }
            var inputModule = eventSystem.currentInputModule;
            if (inputModule == null)
            {
                return false;
            }
            var input = inputModule.input;
            if (input == null)
            {
                mousePosition = Input.mousePosition;
                return Input.GetMouseButtonUp(0);
            }
            else
            {
                mousePosition = input.mousePosition;
                return input.GetMouseButtonUp(0);
            }
        }
        private bool IsPassThroughRaycast(EventSystem eventSystem, Vector2 mousePosition)
        {
            var canvas = _canvas;
            if (canvas == null)
            {
                return false;
            }
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera
                ;
            if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, mousePosition, cam))
            {
                // ヒットしてない
                return false;
            }

            var eventData = new PointerEventData(eventSystem)
            {
                position = mousePosition
            };
            eventSystem.RaycastAll(eventData, _raycastResults);

            if (_raycastResults.Count > 0)
            {
                RaycastResult target = _raycastResults[0];
                if (target.gameObject == gameObject)
                {
                    return true;
                }
                if (target.sortingLayer != canvas.sortingLayerID)
                {
                    return SortingLayer.GetLayerValueFromID(target.sortingLayer) < SortingLayer.GetLayerValueFromID(canvas.sortingLayerID);
                }
                if (target.sortingOrder != canvas.sortingOrder)
                {
                    return target.sortingOrder < canvas.sortingOrder;
                }
                return target.depth < canvasRenderer.absoluteDepth;
            }
            else
            {
                return true;
            }
        }
        private void CacheCanvas()
        {
            var list = ListPool<Canvas>.Get();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count > 0)
            {
                // Find the first active and enabled canvas.
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].isActiveAndEnabled)
                    {
                        _canvas = list[i];
                        break;
                    }

                    // if we reached the end and couldn't find an active and enabled canvas, we should return null . case 1171433
                    if (i == list.Count - 1)
                        _canvas = null;
                }
            }
            else
            {
                _canvas = null;
            }

            ListPool<Canvas>.Release(list);
        }
        protected override void OnCanvasGroupChanged()
        {
            var parentGroupAllowsInteraction = ParentGroupAllowsInteraction();
            if (parentGroupAllowsInteraction != _groupsAllowInteraction)
            {
                _groupsAllowInteraction = parentGroupAllowsInteraction;
            }
        }

        bool ParentGroupAllowsInteraction()
        {
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(_canvasGroupCache);
                for (var i = 0; i < _canvasGroupCache.Count; i++)
                {
                    if (_canvasGroupCache[i].enabled && !_canvasGroupCache[i].interactable)
                        return false;

                    if (_canvasGroupCache[i].ignoreParentGroups)
                        return true;
                }

                t = t.parent;
            }

            return true;
        }
        private static readonly List<RaycastResult> _raycastResults = new();
        private readonly List<CanvasGroup> _canvasGroupCache = new List<CanvasGroup>();
        private bool _groupsAllowInteraction = true;
        private Canvas _canvas;
        private CanvasRenderer _canvasRenderer;
        private bool _isPointerDown;
    }
}