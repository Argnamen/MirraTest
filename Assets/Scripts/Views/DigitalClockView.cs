using System;
using UnityEngine;
using UnityEngine.UI;
using ClockApp.ViewModels;
using Zenject;
using TMPro;

namespace ClockApp.Views
{
    public class DigitalClockView : ClockView
    {
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private InputField hoursInput;
        [SerializeField] private InputField minutesInput;
        [SerializeField] private Button editButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private GameObject editPanel;

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
            // Обновляем текстовое отображение
            if (timeText != null)
                timeText.text = newTime.ToString("HH:mm:ss");

            // Обновляем поля ввода, если они активны
            if (hoursInput != null && !hoursInput.isFocused)
                hoursInput.text = newTime.Hour.ToString("00");

            if (minutesInput != null && !minutesInput.isFocused)
                minutesInput.text = newTime.Minute.ToString("00");
        }

        protected override void OnEditModeChanged(bool isEditing)
        {
            if (editPanel != null)
                editPanel.SetActive(isEditing);

            if (editButton != null)
                editButton.gameObject.SetActive(!isEditing);
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
    }
}