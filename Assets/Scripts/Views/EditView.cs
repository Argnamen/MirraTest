using ClockApp.ViewModels;
using ClockApp.Views;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        [SerializeField] private TMP_InputField timeInput;
        [SerializeField] private TMP_InputField dateInput;

        private DateTime _newTime;

        private bool _isEditing = false;

        [Inject]
        public void Construct(ClockViewModel viewModel)
        {
            Initialize(viewModel);
            SetupUI();

            timeInput.onValueChanged.AddListener(EditTime);
            dateInput.onValueChanged.AddListener(EditDate);

            _newTime = ViewModel.CurrentTime.Value;
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
            _isEditing = !_isEditing;
            ViewModel.SetEditMode(_isEditing);
        }

        private void OnSaveClicked()
        {
            // Используем метод для синхронизации с аналоговыми часами
            ViewModel.SetTimeFromDigital(_newTime);
            ViewModel.SetEditMode(false);

            _isEditing = false;
        }

        private void OnCancelClicked()
        {
            ViewModel.SetEditMode(false);
            // Восстанавливаем текущее время
            OnTimeChanged(ViewModel.CurrentTime.Value);

            _isEditing = false;
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

        private void EditTime(string text)
        {
            if (text == "")
                return;

            text = text.Replace(":", "");

            if (text.Length % 2 != 0)
                text += "0";

            if (text.Length > 6)
                text = text[0..6];

            string date = default;

            if (text.Length < 3)
            {
                date = $"00:00:{text}";
            }
            else if(text.Length < 5)
            {
                date = $"00:{text[0..2]}:{text[2..(text.Length)]}";
            }
            else
            {
                date = $"{text[0..2]}:{text[2..4]}:{text[4..text.Length]}";
            }

            date = TryParseTime(date);

            SaveTime(date);

            timeInput.SetTextWithoutNotify(date);

        }

        private void EditDate(string text)
        {
            if (text == "")
                return;

            text = text.Replace(".", "");

            if (text.Length > 8)
                text = text[0..8];

            string date = default;

            if (text.Length < 3)
            {
                date = $"00.00.{text}";
            }
            else if (text.Length < 5)
            {
                date = $"00.{text[0..2]}.{text[2..(text.Length)]}";
            }
            else
            {
                date = $"{text[0..2]}.{text[2..4]}.{text[4..text.Length]}";
            }

            SaveDate(date);

            dateInput.SetTextWithoutNotify(date);

        }

        private void SaveTime(string newTime)
        {
            var time = DateTime.Parse(newTime);
            _newTime = new DateTime(_newTime.Year, _newTime.Month, _newTime.Day, time.Hour, time.Minute, time.Second);
        }

        private void SaveDate(string newDate)
        {
            var time = DateTime.Parse(newDate);
            _newTime = new DateTime(time.Year, time.Month, time.Day, _newTime.Hour, _newTime.Minute, _newTime.Second);
        }

        private string TryParseTime(string time)
        {
            var date = time;
            var second = Convert.ToInt32(date[6..date.Length]);

            if (second < 0 || second > 59)
                date = $"{date[0..5]}:00";

            var minute = Convert.ToInt32(date[3..5]);

            if (minute < 0 || minute > 59)
                date = $"{date[0..2]}:00:{date[7..date.Length]}";

            var hour = Convert.ToInt32(date[0..2]);

            if (hour < 0 || hour > 23)
                date = $"00:{date[3..date.Length]}";

            return date;
        }

        private void OpenAnimate()
        {
            Sequence sequence = DOTween.Sequence();
            //sequence.AppendCallback(() => editButton.gameObject.SetActive(false));
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
            //sequence.AppendCallback(() => editButton.gameObject.SetActive(true));
        }
    }
}
