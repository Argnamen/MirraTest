using System;
using UnityEngine;
using ClockApp.ViewModels;
using ClockApp.Views.Components;
using DG.Tweening;
using Zenject;

namespace ClockApp.Views
{
    public class AnalogClockView : ClockView
    {
        [Header("Clock Hands")]
        [SerializeField] private Transform hourHand;
        [SerializeField] private Transform minuteHand;
        [SerializeField] private Transform secondHand;

        [Header("Draggers")]
        [SerializeField] private AngularClockHandDragger hourHandDragger;
        [SerializeField] private AngularClockHandDragger minuteHandDragger;

        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private bool smoothAnimation = true;

        private bool _isDragging = false;

        [Inject]
        public void Construct(ClockViewModel viewModel)
        {
            Initialize(viewModel);
            SetupDraggers();
        }

        private void SetupDraggers()
        {
            if (hourHandDragger != null)
            {
                hourHandDragger.Initialize(ViewModel);
                hourHandDragger.OnDragStarted += OnDragStarted;
                hourHandDragger.OnDragEnded += OnDragEnded;
            }

            if (minuteHandDragger != null)
            {
                minuteHandDragger.Initialize(ViewModel);
                minuteHandDragger.OnDragStarted += OnDragStarted;
                minuteHandDragger.OnDragEnded += OnDragEnded;
            }
        }

        private void OnDragStarted()
        {
            _isDragging = true;
            StopAnimations();
        }

        private void OnDragEnded()
        {
            _isDragging = false;
        }

        protected override void OnTimeChanged(DateTime newTime)
        {
            UpdateClockHands(newTime);
        }

        protected override void OnEditModeChanged(bool isEditing)
        {
            // Включаем/выключаем возможность перетаскивания
            if (hourHandDragger != null) hourHandDragger.enabled = isEditing;
            if (minuteHandDragger != null) minuteHandDragger.enabled = isEditing;

            // Визуально подсвечиваем активные стрелки
            HighlightActiveHands(isEditing);
        }

        private void UpdateClockHands(DateTime time)
        {
            float secondsRotation = -time.Second * 6f;
            float minutesRotation = -(time.Minute * 6f + time.Second * 0.1f);
            float hoursRotation = -((time.Hour % 12) * 30f + time.Minute * 0.5f);

            if (smoothAnimation && !_isDragging)
            {
                secondHand.DORotate(new Vector3(0, 0, secondsRotation), animationDuration);
                minuteHand.DORotate(new Vector3(0, 0, minutesRotation), animationDuration);
                hourHand.DORotate(new Vector3(0, 0, hoursRotation), animationDuration);
            }
            else
            {
                secondHand.rotation = Quaternion.Euler(0, 0, secondsRotation);
                minuteHand.rotation = Quaternion.Euler(0, 0, minutesRotation);
                hourHand.rotation = Quaternion.Euler(0, 0, hoursRotation);
            }
        }

        private void StopAnimations()
        {
            DOTween.Kill(hourHand);
            DOTween.Kill(minuteHand);
            DOTween.Kill(secondHand);
        }

        private void HighlightActiveHands(bool isActive)
        {
            // Визуальная индикация активных стрелок
            // Можно изменить цвет, добавить свечение и т.д.
        }

        private void OnDestroy()
        {
            if (hourHandDragger != null)
            {
                hourHandDragger.OnDragStarted -= OnDragStarted;
                hourHandDragger.OnDragEnded -= OnDragEnded;
            }

            if (minuteHandDragger != null)
            {
                minuteHandDragger.OnDragStarted -= OnDragStarted;
                minuteHandDragger.OnDragEnded -= OnDragEnded;
            }
        }
    }
}
