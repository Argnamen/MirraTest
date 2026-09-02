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
        private readonly TimeDataModel _timeDataModel;
        private float _accumulatedTime;
        private bool _isDragging;

        public ObservableProperty<DateTime> CurrentTime { get; }
        public ObservableProperty<bool> IsLoading { get; }
        public ObservableProperty<string> ErrorMessage { get; }
        public ObservableProperty<bool> IsEditMode { get; }
        public ObservableProperty<bool> IsDragging { get; }
        public ObservableProperty<bool> IsTimeSynced { get; }

        [Inject]
        public ClockViewModel(
            TimeModel timeModel,
            ITimeService timeService,
            TimeDataModel timeDataModel)
        {
            _timeModel = timeModel;
            _timeService = timeService;
            _timeDataModel = timeDataModel;

            CurrentTime = new ObservableProperty<DateTime>();
            IsLoading = new ObservableProperty<bool>(false);
            ErrorMessage = new ObservableProperty<string>();
            IsEditMode = new ObservableProperty<bool>(false);
            IsDragging = new ObservableProperty<bool>(false);
            IsTimeSynced = new ObservableProperty<bool>(false);
        }

        public void Initialize()
        {
            // Проверяем, есть ли время из GameScene
            if (_timeDataModel != null && _timeDataModel.IsTimeLoaded)
            {
                // Устанавливаем время в TimeModel
                _timeModel.SetInitialTime(_timeDataModel.ServerTime);

                // Обновляем CurrentTime
                CurrentTime.Value = _timeDataModel.ServerTime;
                IsTimeSynced.Value = true;

                Debug.Log($"Часы инициализированы временем: {_timeDataModel.ServerTime:HH:mm:ss}");
            }
            else
            {
                // Время не загружено, запрашиваем с сервера
                RequestTimeFromServer();
            }
        }

        public void RequestTimeFromServer()
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
            if (!IsEditMode.Value && !IsLoading.Value && !IsDragging.Value && IsTimeSynced.Value)
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

        public void SetTime(DateTime newTime)
        {
            _timeModel.UpdateTime(newTime);
            CurrentTime.Value = newTime;
            IsTimeSynced.Value = true;
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

        public void SetTimeFromAnalog(DateTime newTime)
        {
            if (!IsEditMode.Value) return;

            SetTime(newTime);
        }

        public void SetTimeFromDigital(DateTime newTime)
        {
            SetTime(newTime);
        }

        public void StartDragging()
        {
            IsDragging.Value = true;
        }

        public void StopDragging()
        {
            IsDragging.Value = false;
        }

        public void Dispose()
        {
            // Очистка ресурсов
        }
    }
}