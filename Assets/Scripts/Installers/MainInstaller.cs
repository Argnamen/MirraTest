using UnityEngine;
using Zenject;
using ClockApp.Models;
using ClockApp.Services;
using ClockApp.ViewModels;
using ClockApp.Views;

namespace ClockApp.Installers
{
    public class MainInstaller : MonoInstaller
    {
        [Header("Views")]
        [SerializeField] private AnalogClockView analogClockView;
        [SerializeField] private DigitalClockView digitalClockView;

        [Header("Settings")]
        [SerializeField] private string gameSceneAddress = "GameScene";
        public override void InstallBindings()
        {
            //Container.Bind<AddressableManager>()
                //.AsSingle();

            // Bind Models
            Container.BindInterfacesAndSelfTo<TimeModel>()
                .AsSingle().NonLazy();

            // Bind Services
            Container.Bind<ITimeService>()
                .To<TimeService>()
                .AsSingle().NonLazy();

            Container.Bind<ISceneLoaderService>()
                .To<SceneLoaderService>()
                .AsSingle().NonLazy();

            // Bind Views
            Container.BindInterfacesAndSelfTo<AnalogClockView>()
                .FromInstance(analogClockView)
                .AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<DigitalClockView>()
                .FromInstance(digitalClockView)
                .AsSingle().NonLazy();

            // Bind MainController
            Container.BindInterfacesAndSelfTo<MainController>()
                .AsSingle().NonLazy();

            // Bind ViewModels
            Container.BindInterfacesAndSelfTo<ClockViewModel>()
                .AsSingle().NonLazy();

            // Bind Addressable settings
            //Container.Bind<string>()
            //.WithId("GameSceneAddress")
            //.FromInstance(gameSceneAddress)
            //.AsCached();
        }
    }
}
