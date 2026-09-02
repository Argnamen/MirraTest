using ClockApp.Core;
using ClockApp.Models;
using ClockApp.Services;
using ClockApp.ViewModels;
using ClockApp.Views;
using UnityEngine;
using Zenject;

namespace ClockApp.Installers
{
    public class MainInstaller : MonoInstaller
    {
        [Header("Views")]
        [SerializeField] private AnalogClockView analogClockView;
        [SerializeField] private DigitalClockView digitalClockView;

        public override void InstallBindings()
        {
            // ѕолучаем данные из GameScene
            var timeData = SceneDataTransfer.GetTimeData();

            // TimeDataModel
            Container.BindInterfacesAndSelfTo<TimeDataModel>()
                .FromInstance(timeData)
                .AsSingle()
                .NonLazy();

            // TimeModel - просто биндим, без параметров
            Container.BindInterfacesAndSelfTo<TimeModel>()
                .AsSingle()
                .NonLazy();

            // Services
            Container.Bind<ITimeService>()
                .To<TimeService>()
                .AsSingle()
                .NonLazy();

            // ViewModels
            Container.BindInterfacesAndSelfTo<ClockViewModel>()
                .AsSingle()
                .NonLazy();

            // Views
            Container.BindInterfacesAndSelfTo<AnalogClockView>()
                .FromInstance(analogClockView)
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<DigitalClockView>()
                .FromInstance(digitalClockView)
                .AsSingle()
                .NonLazy();
        }
    }
}