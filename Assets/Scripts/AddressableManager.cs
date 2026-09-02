using UnityEngine;
using ClockApp.Services;
using Zenject;

namespace ClockApp
{
    public class AddressableManager : IInitializable
    {
        private readonly ISceneLoaderService _sceneLoader;
        private readonly string _gameSceneAddress;
        private GameObject _loadingScreen;
        public AddressableManager(
            ISceneLoaderService sceneLoader,
            [Inject(Id = "GameSceneAddress")] string gameSceneAddress)
        {
            _sceneLoader = sceneLoader;
            _gameSceneAddress = gameSceneAddress;
        }

        public void Initialize()
        {
            LoadGameScene();
        }

        private void LoadGameScene()
        {
            if (_loadingScreen != null)
                _loadingScreen.SetActive(true);

            _sceneLoader.LoadSceneAsync(
                _gameSceneAddress,
                onSuccess: () =>
                {
                    if (_loadingScreen != null)
                        _loadingScreen.SetActive(false);
                },
                onError: (error) =>
                {
                    Debug.LogError($"Не удалось загрузить сцену: {error}");
                    if (_loadingScreen != null)
                        _loadingScreen.SetActive(false);
                }
            );
        }
    }
}
