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
        public MainController(
            ClockViewModel viewModel,
            AnalogClockView analogClockView,
            DigitalClockView digitalClockView)
        {
            _viewModel = viewModel;
            _analogClockView = analogClockView;
            _digitalClockView = digitalClockView;
        }

        public void Initialize()
        {
            _analogClockView.Initialize(_viewModel);
            _digitalClockView.Initialize(_viewModel);

            // ViewModel инициализируется автоматически через Zenject
        }

        public void Dispose()
        {
            _viewModel.Dispose();
        }
    }
}
