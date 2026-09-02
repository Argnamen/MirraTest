using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using ClockApp.Models;
using Zenject;

namespace ClockApp.Services
{
    public class TimeService : ITimeService
    {
        private const string TimeApiUrl = "https://yandex.com/time/sync.json";

        public IEnumerator GetServerTime(Action<DateTime> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(TimeApiUrl))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string json = request.downloadHandler.text;
                        long unixTime = ParseUnixTime(json);
                        DateTime serverTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).LocalDateTime;
                        onSuccess?.Invoke(serverTime);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"Ошибка парсинга времени: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"Ошибка запроса: {request.error}");
                }
            }
        }

        private long ParseUnixTime(string json)
        {
            var timeData = JsonUtility.FromJson<TimeResponse>(json);
            return timeData.time;
        }

        [Serializable]
        private class TimeResponse
        {
            public long time;
        }
    }
}

