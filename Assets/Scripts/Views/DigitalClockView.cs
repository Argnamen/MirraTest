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
        }

        private void OnEnable()
        {
            StartAnim();
        }

        private void StartAnim()
        {
            Vector3 startDatePos = dateContainer.localPosition;
            Vector3 startEditPos = editContainer.localPosition;

            Debug.Log(startDatePos);

            dateContainer.localPosition = timeContainer.localPosition;
            editContainer.localPosition = timeContainer.localPosition;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(baseContainer.transform.DORotate(new Vector3(0, 0, 105), 0.1f));
            sequence.Append(baseContainer.transform.DORotate(Vector3.zero, 1f));
            sequence.Join(baseContainer.DOFade(1f, 0.8f));
            sequence.Join(dateContainer.DOLocalMoveY(startDatePos.y, 1f));
            sequence.Join(editContainer.DOLocalMoveY(startEditPos.y, 1f));
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