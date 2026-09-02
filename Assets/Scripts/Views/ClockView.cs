using ClockApp.ViewModels;
using System;
using UnityEngine;

namespace ClockApp.Views
{
    public abstract class ClockView : MonoBehaviour
    {
        protected ClockViewModel ViewModel;

        public virtual void Initialize(ClockViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.CurrentTime.OnValueChanged += OnTimeChanged;
            ViewModel.IsEditMode.OnValueChanged += OnEditModeChanged;
            ViewModel.IsLoading.OnValueChanged += OnLoadingChanged;
        }

        protected virtual void Start() { }

        protected virtual void OnTimeChanged(DateTime newTime) { }
        protected virtual void OnEditModeChanged(bool isEditing) { }
        protected virtual void OnLoadingChanged(bool isLoading) { }

        protected virtual void OnDestroy()
        {
            if (ViewModel != null)
            {
                ViewModel.CurrentTime.OnValueChanged -= OnTimeChanged;
                ViewModel.IsEditMode.OnValueChanged -= OnEditModeChanged;
                ViewModel.IsLoading.OnValueChanged -= OnLoadingChanged;
            }
        }
    }
}
