using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace ClockApp.Services
{
    public class SceneLoader
    {
        public async Task LoadSceneAsync(string address, Action<float> onProgress = null)
        {
            try
            {
                Debug.Log($"Начинаем загрузку сцены: {address}");

                // Загружаем сцену
                var handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Single);

                // Ждем завершения загрузки
                while (!handle.IsDone)
                {
                    // Отправляем прогресс
                    onProgress?.Invoke(handle.PercentComplete);
                    await Task.Yield();
                }

                // Проверяем результат
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Сцена {address} успешно загружена");
                    onProgress?.Invoke(1f);
                }
                else
                {
                    Debug.LogError($"Ошибка загрузки сцены: {handle.OperationException}");
                    throw new Exception($"Ошибка загрузки: {handle.OperationException}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Исключение: {e.Message}");
                throw;
            }
        }
    }
}
