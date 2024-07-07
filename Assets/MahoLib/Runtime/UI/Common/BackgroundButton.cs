using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Maho.UI.Common
{
    /// <summary>
    /// 背景ボタン
    /// </summary>
    public class BackgroundButton
        : UIBehaviour
        , IPointerClickHandler
        , IPointerDownHandler
        , IPointerUpHandler
    {
        [SerializeField]
        private bool _interactable = true;

        [SerializeField]
        private bool _bubble = false;

        [SerializeField]
        private UnityEvent _onClick = new UnityEvent();

        public bool interactable
        {
            get => _interactable;
            set => _interactable = value;
        }
        public UnityEvent onClick
        {
            get => _onClick;
            set => _onClick = value;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            if (!IsActive() || !interactable)
            {
                return;
            }
            _onClick?.Invoke();

            if (_bubble)
            {
                // 後ろもクリック
                if (_bubbleTarget != null)
                {
                    ExecuteEvents.Execute(_bubbleTarget, eventData, ExecuteEvents.pointerClickHandler);
                }
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_bubble)
            {
                // 後ろもクリック
                _bubbleTarget = GetBubbleTarget(eventData);
                if (_bubbleTarget != null)
                {
                    ExecuteEvents.Execute(_bubbleTarget, eventData, ExecuteEvents.pointerDownHandler);
                }
            }
            else
            {
                _bubbleTarget = null;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_bubble)
            {
                if (_bubbleTarget != null)
                {
                    ExecuteEvents.Execute(_bubbleTarget, eventData, ExecuteEvents.pointerUpHandler);
                }
            }
        }
        public void SetOnClick(UnityAction action)
        {
            _onClick.RemoveAllListeners();
            _onClick.AddListener(action);
        }

        GameObject GetBubbleTarget(PointerEventData eventData)
        {
            GameObject target = null;
            EventSystem.current.RaycastAll(eventData, _raycastResults);
            bool isSkip = true;
            foreach (var ray in _raycastResults)
            {
                if (ray.gameObject == this.gameObject)
                {
                    isSkip = false;
                    continue;
                }
                if (isSkip)
                {
                    continue;
                }
                if (ray.gameObject == null)
                {
                    continue;
                }
                target = ray.gameObject;
                break;
            }
            _raycastResults.Clear();
            return target;
        }
        List<RaycastResult> _raycastResults = new();
        GameObject _bubbleTarget;
    }
}