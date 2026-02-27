using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIEventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Action<PointerEventData> OnClickHandler = null;
        public Action<PointerEventData> OnPressedHandler = null;
        public Action<PointerEventData> OnPointerDownHandler = null;
        public Action<PointerEventData> OnPointerUpHandler = null;
        public Action<PointerEventData> OnDragHandler = null;
        public Action<PointerEventData> OnBeginDragHandler = null;
        public Action<PointerEventData> OnEndDragHandler = null;
        public Action<PointerEventData> OnPointerEnteredHandler = null;
        public Action<PointerEventData> OnPointerExitHandler = null;

        private PointerEventData _lastEventData;
        private bool _isPressed = false;

        private void Update()
        {
            if (_isPressed)
            {
                OnPressedHandler?.Invoke(_lastEventData);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickHandler?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            OnPointerDownHandler?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
            OnPointerUpHandler?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragHandler?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragHandler?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragHandler?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnteredHandler?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitHandler?.Invoke(eventData);
        }
    }
}