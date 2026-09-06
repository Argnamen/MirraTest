using ClockApp.ViewModels;
using ClockApp.Views;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Clock.App.View
{
    public class EditView : ClockView
    {
        [SerializeField] private CanvasGroup editPanel;
        [SerializeField] private Image timeFaceImage;
        [SerializeField] private InputField hoursInput;
        [SerializeField] private InputField minutesInput;
        [SerializeField] private Button editButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;

        [Inject]
        public void Construct(ClockViewModel viewModel)
        {
            Initialize(viewModel);
            SetupUI();
        }

        private void SetupUI()
        {
            if (editButton != null)
                editButton.onClick.AddListener(OnEditClicked);

            if (saveButton != null)
                saveButton.onClick.AddListener(OnSaveClicked);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);

            if (hoursInput != null)
                hoursInput.onEndEdit.AddListener(OnInputEndEdit);

            if (minutesInput != null)
                minutesInput.onEndEdit.AddListener(OnInputEndEdit);
        }

        protected override void OnTimeChanged(DateTime newTime)
        {
            // Обновляем поля ввода, если они активны
            if (hoursInput != null && !hoursInput.isFocused)
                hoursInput.text = newTime.Hour.ToString("00");

            if (minutesInput != null && !minutesInput.isFocused)
                minutesInput.text = newTime.Minute.ToString("00");
        }
        protected override void OnEditModeChanged(bool isEditing)
        {
            if (isEditing)
            {
                OpenAnimate();
            }
            else
            {
                CloseAnimate();
            }
        }

        private void OnEditClicked()
        {
            ViewModel.SetEditMode(true);
        }

        private void OnSaveClicked()
        {
            if (TryParseTime(out DateTime newTime))
            {
                // Используем метод для синхронизации с аналоговыми часами
                ViewModel.SetTimeFromDigital(newTime);
                ViewModel.SetEditMode(false);
            }
        }

        private bool TryParseTime(out DateTime time)
        {
            time = default;

            if (int.TryParse(hoursInput.text, out int hours) &&
                int.TryParse(minutesInput.text, out int minutes))
            {
                if (hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60)
                {
                    time = ViewModel.CurrentTime.Value.Date.AddHours(hours).AddMinutes(minutes);
                    return true;
                }
            }

            return false;
        }

        private void OnCancelClicked()
        {
            ViewModel.SetEditMode(false);
            // Восстанавливаем текущее время
            OnTimeChanged(ViewModel.CurrentTime.Value);
        }

        private void OnInputEndEdit(string value)
        {
            // Валидация ввода
            if (int.TryParse(value, out int number))
            {
                if (number < 0)
                {
                    if (hoursInput != null && hoursInput.isFocused)
                        hoursInput.text = "00";
                    else if (minutesInput != null && minutesInput.isFocused)
                        minutesInput.text = "00";
                }
                else if (number > 23 && hoursInput != null && hoursInput.isFocused)
                {
                    hoursInput.text = "23";
                }
                else if (number > 59 && minutesInput != null && minutesInput.isFocused)
                {
                    minutesInput.text = "59";
                }
            }
        }

        private void OpenAnimate()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => editButton.gameObject.SetActive(false));
            sequence.AppendCallback(() => editPanel.gameObject.SetActive(true));
            sequence.Append(timeFaceImage.DOFade(0, 1f));
            sequence.Join(timeFaceImage.transform.DOScale(5f, 2f));
            sequence.Join(editPanel.DOFade(1, 1f));

            sequence.AppendCallback(() => timeFaceImage.transform.localScale = Vector3.one);
            sequence.Append(timeFaceImage.DOFade(1, 0.1f));
        }
        private void CloseAnimate()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(editPanel.DOFade(0, 0.5f));
            sequence.AppendCallback(() => editPanel.gameObject.SetActive(false));
            sequence.AppendCallback(() => editButton.gameObject.SetActive(true));
        }
    }
}
