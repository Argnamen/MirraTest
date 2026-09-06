using System;
using UnityEngine;
using UnityEngine.UI;
using ClockApp.ViewModels;
using Zenject;
using TMPro;
using DG.Tweening;

namespace ClockApp.Views
{
    public class DigitalClockView : ClockView
    {
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text dateText;
        [SerializeField] private GameObject editPanel;

        [SerializeField] private CanvasGroup baseContainer;
        [SerializeField] private Transform timeContainer;
        [SerializeField] private Transform dateContainer;
        [SerializeField] private Transform editContainer;

        [Inject]
        public void Construct(ClockViewModel viewModel)
        {
            Initialize(viewModel);
        }

        public override void Initialize(ClockViewModel viewModel)
        {
            base.Initialize(viewModel);

            StartAnim();
        }

        private void StartAnim()
        {
            Vector3 startDatePos = dateContainer.transform.localPosition;
            Vector3 startEditPos = editContainer.transform.localPosition;

            dateContainer.transform.localPosition = timeContainer.transform.localPosition;
            editContainer.transform.localPosition = timeContainer.transform.localPosition;

            baseContainer.transform.DORotate(Vector3.zero, 1f);
            baseContainer.DOFade(1f, 0.8f);

            dateContainer.DOLocalMove(startDatePos, 1f);
            editContainer.DOLocalMove(startEditPos, 1f);
        }

        protected override void OnTimeChanged(DateTime newTime)
        {
            // Обновляем текстовое отображение
            if (timeText != null)
                timeText.text = newTime.ToString("HH:mm:ss");
            if (dateText != null)
                dateText.text = newTime.ToString("dd.MM.yyyy"); 
        }
    }
}