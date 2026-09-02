using UnityEngine;
using Zenject;
using ClockApp.Services;

namespace ClockApp.GameScene
{
    public class GameSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Биндим SceneLoader
            Container.Bind<SceneLoader>()
                .AsSingle()
                .NonLazy();

            // Биндим контроллер
            Container.Bind<GameSceneController>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}
