using System;
using UnityEngine;
using UnityEngine.EventSystems;
using ClockApp.ViewModels;

namespace ClockApp.Views.Components
{
    public class AngularClockHandDragger : MonoBehaviour,
        IDragHandler,
        IBeginDragHandler,
        IEndDragHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        [Header("Settings")]
        [SerializeField] private ClockHandType handType;
        [SerializeField] private bool invertRotation = false;

        [Header("References")]
        [SerializeField] private RectTransform clockFaceRect;
        [SerializeField] private Camera uiCamera;

        private ClockViewModel _viewModel;
        private bool _isDragging;
        private DateTime _startTime;
        private float _startHandAngle;
        private float _startPointerAngle;
        private float _angleDelta = 0;

        public event Action<DateTime> OnTimeChanged;
        public event Action OnDragStarted;
        public event Action OnDragEnded;

        public enum ClockHandType
        {
            Hour,
            Minute,
            Second
        }

        public void Initialize(ClockViewModel viewModel)
        {
            _viewModel = viewModel;

            if (uiCamera == null)
            {
                Canvas canvas = GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                    {
                        uiCamera = canvas.worldCamera;
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_viewModel == null || !_viewModel.IsEditMode.Value) return;

            _startTime = _viewModel.CurrentTime.Value;
            _startHandAngle = GetCurrentHandAngle();
            _startPointerAngle = GetPointerAngle(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_viewModel == null || !_viewModel.IsEditMode.Value) return;

            _isDragging = true;

            _startTime = _viewModel.CurrentTime.Value;
            _startHandAngle = GetCurrentHandAngle();
            _startPointerAngle = GetPointerAngle(eventData);

            _viewModel.StartDragging();

            OnDragStarted?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || _viewModel == null) return;

            float currentPointerAngle = GetPointerAngle(eventData);

            if (invertRotation)
                _angleDelta -= Mathf.DeltaAngle(_startPointerAngle, currentPointerAngle);

            DateTime newTime = CalculateTimeFromAngle(_startTime, _angleDelta);

            if(newTime.Hour != _viewModel.CurrentTime.Value.Hour ||
                newTime.Minute != _viewModel.CurrentTime.Value.Minute)
                    {
                        _angleDelta = 0;
                    }
            
            _viewModel.SetTimeFromAnalog(newTime);

            Debug.Log(_angleDelta);

            OnTimeChanged?.Invoke(newTime);

            _startTime = newTime;

            _startPointerAngle = currentPointerAngle;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            _isDragging = false;
            _viewModel.StopDragging();

            OnDragEnded?.Invoke();
        }

        private float GetPointerAngle(PointerEventData eventData)
        {
            Vector2 localPoint;

            if (clockFaceRect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    clockFaceRect,
                    eventData.position,
                    uiCamera,
                    out localPoint
                );
            }
            else
            {
                // ���������� ������������ RectTransform
                RectTransform parentRect = transform.parent as RectTransform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    eventData.position,
                    uiCamera,
                    out localPoint
                );
            }

            float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;

            angle += 90f;

            return angle;
        }

        private float GetCurrentHandAngle()
        {
            float angle = -clockFaceRect.localEulerAngles.z;
            return angle;
        }

        private DateTime CalculateTimeFromAngle(DateTime startTime, float angleDelta)
        {
            switch (handType)
            {
                case ClockHandType.Hour:
                    float hoursDelta = angleDelta / 30f;
                    return startTime.AddHours((int)hoursDelta);

                case ClockHandType.Minute:
                    float minutesDelta = angleDelta / 6f;
                    return startTime.AddMinutes((int)minutesDelta);

                case ClockHandType.Second:
                    float secondsDelta = angleDelta / 6f;
                    return startTime.AddSeconds((int)secondsDelta);

                default:
                    return startTime;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}
