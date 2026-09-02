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

            // Если камера не назначена, пробуем найти
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

            // Сохраняем начальные значения
            _startTime = _viewModel.CurrentTime.Value;
            _startHandAngle = GetCurrentHandAngle();
            _startPointerAngle = GetPointerAngle(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_viewModel == null || !_viewModel.IsEditMode.Value) return;

            _isDragging = true;

            // Обновляем начальные значения
            _startTime = _viewModel.CurrentTime.Value;
            _startHandAngle = GetCurrentHandAngle();
            _startPointerAngle = GetPointerAngle(eventData);

            _viewModel.StartDragging();

            OnDragStarted?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || _viewModel == null) return;

            // Получаем текущий угол указателя
            float currentPointerAngle = GetPointerAngle(eventData);

            // Вычисляем разницу углов
            float angleDelta = Mathf.DeltaAngle(_startPointerAngle, currentPointerAngle);

            if (invertRotation)
                angleDelta = -angleDelta;

            // Новое время на основе изменения угла
            DateTime newTime = CalculateTimeFromAngle(_startTime, angleDelta);

            if (newTime < _viewModel.CurrentTime.Value)
            {
                _startTime = _viewModel.CurrentTime.Value;
                newTime = CalculateTimeFromAngle(_startTime, angleDelta);
                _startPointerAngle = currentPointerAngle;
            }

            _viewModel.SetTimeFromAnalog(newTime);

            Debug.Log(newTime.ToString());

            // Обновляем ViewModel

            OnTimeChanged?.Invoke(newTime);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            _isDragging = false;
            _viewModel.StopDragging();

            OnDragEnded?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Дополнительная обработка
        }

        private float GetPointerAngle(PointerEventData eventData)
        {
            Vector2 localPoint;

            // Конвертируем позицию указателя в локальные координаты часов
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
                // Используем родительский RectTransform
                RectTransform parentRect = transform.parent as RectTransform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    eventData.position,
                    uiCamera,
                    out localPoint
                );
            }

            // Вычисляем угол от центра (pivot часов)
            // Atan2(y, x) дает угол от оси X
            float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;

            // Корректируем угол так, чтобы 0 был на 12 часах
            angle += 90f;

            return angle;
        }

        private float GetCurrentHandAngle()
        {
            // Получаем текущий угол стрелки
            float angle = -clockFaceRect.localEulerAngles.z;
            return angle;
        }

        private DateTime CalculateTimeFromAngle(DateTime startTime, float angleDelta)
        {
            switch (handType)
            {
                case ClockHandType.Hour:
                    // 30 градусов = 1 час
                    float hoursDelta = angleDelta / 30f;
                    return startTime.AddHours(hoursDelta);

                case ClockHandType.Minute:
                    // 6 градусов = 1 минута
                    float minutesDelta = angleDelta / 6f;
                    return startTime.AddMinutes(minutesDelta);

                case ClockHandType.Second:
                    // 6 градусов = 1 секунда
                    float secondsDelta = angleDelta / 6f;
                    return startTime.AddSeconds(secondsDelta);

                default:
                    return startTime;
            }
        }
    }
}
