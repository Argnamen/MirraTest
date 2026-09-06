using Clock.App.View;
using ClockApp.ViewModels;
using ClockApp.Views;
using System;
using UnityEngine;
using Zenject;

namespace ClockApp
{
    public class MainController : IInitializable, IDisposable
    {
        private readonly ClockViewModel _viewModel;
        private readonly AnalogClockView _analogClockView;
        private readonly DigitalClockView _digitalClockView;
        private readonly EditView _editView;
        public MainController(
            ClockViewModel viewModel,
            AnalogClockView analogClockView,
            DigitalClockView digitalClockView,
            EditView editView)
        {
            _viewModel = viewModel;
            _analogClockView = analogClockView;
            _digitalClockView = digitalClockView;
            _editView = editView;
        }

        public void Initialize()
        {
            _analogClockView.Initialize(_viewModel);
            _digitalClockView.Initialize(_viewModel);
            _editView.Initialize(_viewModel);

            // ViewModel инициализируется автоматически через Zenject
        }

        public void Dispose()
        {
            _viewModel.Dispose();
        }
    }
}
