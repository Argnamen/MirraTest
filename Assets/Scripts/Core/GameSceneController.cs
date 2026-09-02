using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ClockApp.Services;
using Zenject;
using TMPro;

namespace ClockApp.GameScene
{
    public class GameSceneController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text statusText;

        [Header("Settings")]
        [SerializeField] private string mainSceneAddress = "MainScene";

        private SceneLoader _sceneLoader;

        [Inject]
        public void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private async void Start()
        {
            await LoadMainScene();
        }

        private async Task LoadMainScene()
        {
            try
            {
                // Показываем загрузку
                ShowLoading();

                // Загружаем сцену
                await _sceneLoader.LoadSceneAsync(mainSceneAddress, OnProgress);

                // Скрываем загрузку
                HideLoading();
            }
            catch (Exception e)
            {
                Debug.LogError($"Не удалось загрузить сцену: {e.Message}");
                statusText.text = $"Ошибка: {e.Message}";
            }
        }

        private void OnProgress(float progress)
        {
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (statusText != null)
            {
                statusText.text = $"Загрузка: {Mathf.Round(progress * 100)}%";
            }
        }

        private void ShowLoading()
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(true);

            if (statusText != null)
                statusText.text = "Загрузка...";
        }

        private void HideLoading()
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }
    }
}
