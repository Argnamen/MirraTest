using System;

namespace ClockApp.Services
{
    public interface ISceneLoaderService
    {
        void LoadSceneAsync(string sceneAddress, Action onSuccess = null, Action<string> onError = null);
    }
}
