using System;
using ClockApp.Models;

namespace ClockApp.Core
{
    public static class SceneDataTransfer
    {
        private static TimeDataModel _timeData;

        public static void SetTimeData(TimeDataModel timeData)
        {
            _timeData = timeData;
        }

        public static TimeDataModel GetTimeData()
        {
            if (_timeData == null)
            {
                _timeData = new TimeDataModel();
                _timeData.SetServerTime(DateTime.Now);
            }

            return _timeData;
        }

        public static void Clear()
        {
            _timeData = null;
        }
    }
}