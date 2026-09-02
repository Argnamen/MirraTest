using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace ClockApp.Services
{
    public class SceneLoaderService : ISceneLoaderService
    {
        public async void LoadSceneAsync(string sceneAddress, Action onSuccess = null, Action<string> onError = null)
        {
            try
            {
                // Убеждаемся, что Addressables инициализированы
                await Addressables.InitializeAsync().Task;

                // Загружаем сцену асинхронно
                var handle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Сцена '{sceneAddress}' успешно загружена");
                    onSuccess?.Invoke();
                }
                else
                {
                    onError?.Invoke($"Ошибка загрузки сцены: {handle.OperationException}");
                }
            }
            catch (Exception e)
            {
                onError?.Invoke($"Исключение при загрузке: {e.Message}");
            }
        }
    }
}
