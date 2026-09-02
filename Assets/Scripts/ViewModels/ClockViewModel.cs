using System;
using UnityEngine;
using ClockApp.Core;
using ClockApp.Models;
using ClockApp.Services;
using Zenject;

namespace ClockApp.ViewModels
{
    public class ClockViewModel : IInitializable, IDisposable, ITickable
    {
        private readonly TimeModel _timeModel;
        private readonly ITimeService _timeService;
        private float _accumulatedTime;
        private bool _isDragging;

        public ObservableProperty<DateTime> CurrentTime { get; }
        public ObservableProperty<bool> IsLoading { get; }
        public ObservableProperty<string> ErrorMessage { get; }
        public ObservableProperty<bool> IsEditMode { get; }
        public ObservableProperty<bool> IsDragging { get; }

        // События для синхронизации
        public event Action<DateTime> OnTimeChangedFromAnalog;
        public event Action<DateTime> OnTimeChangedFromDigital;
        public event Action OnDragStarted;
        public event Action OnDragEnded;

        [Inject]
        public ClockViewModel(TimeModel timeModel, ITimeService timeService)
        {
            _timeModel = timeModel;
            _timeService = timeService;

            CurrentTime = new ObservableProperty<DateTime>();
            IsLoading = new ObservableProperty<bool>(false);
            ErrorMessage = new ObservableProperty<string>();
            IsEditMode = new ObservableProperty<bool>(false);
            IsDragging = new ObservableProperty<bool>(false);

            // Подписываемся на изменения времени
            CurrentTime.OnValueChanged += OnCurrentTimeChanged;
        }

        public void Initialize()
        {
            _timeService.GetServerTime(
                onSuccess: (serverTime) =>
                {
                    _timeModel.UpdateTime(serverTime);
                    CurrentTime.Value = serverTime;
                    IsLoading.Value = false;
                },
                onError: (error) =>
                {
                    Debug.LogWarning($"Не удалось получить время с сервера: {error}. Используем локальное время.");
                    _timeModel.UpdateTime(DateTime.Now);
                    CurrentTime.Value = _timeModel.CurrentTime;
                    IsLoading.Value = false;
                    ErrorMessage.Value = error;
                }
            );
        }

        public void Tick()
        {
            if (!IsEditMode.Value && !IsLoading.Value && !IsDragging.Value)
            {
                _accumulatedTime += Time.deltaTime;
                if (_accumulatedTime >= 1f)
                {
                    _timeModel.Tick(TimeSpan.FromSeconds(_accumulatedTime));
                    CurrentTime.Value = _timeModel.CurrentTime;
                    _accumulatedTime = 0f;
                }
            }
        }

        private void OnCurrentTimeChanged(DateTime newTime)
        {
            // Автоматически уведомляем все View об изменении
            Debug.Log($"Время обновлено: {newTime:HH:mm:ss}");
        }

        public void SetEditMode(bool isEditing)
        {
            IsEditMode.Value = isEditing;
            if (isEditing)
            {
                StartDragging();
            }
            else
            {
                StopDragging();
            }
        }

        public void SetTime(DateTime newTime)
        {
            _timeModel.UpdateTime(newTime);
            CurrentTime.Value = newTime;
        }

        // Методы для аналоговых часов
        public void SetTimeFromAnalog(DateTime newTime)
        {
            if (!IsEditMode.Value) return;

            SetTime(newTime);
            OnTimeChangedFromAnalog?.Invoke(newTime);
        }

        // Методы для цифровых часов
        public void SetTimeFromDigital(DateTime newTime)
        {
            SetTime(newTime);
            OnTimeChangedFromDigital?.Invoke(newTime);
        }

        public void StartDragging()
        {
            IsDragging.Value = true;
            OnDragStarted?.Invoke();
        }

        public void StopDragging()
        {
            IsDragging.Value = false;
            OnDragEnded?.Invoke();
        }

        public void Dispose()
        {
            CurrentTime.OnValueChanged -= OnCurrentTimeChanged;
        }
    }
}