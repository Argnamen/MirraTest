using System;

namespace ClockApp.Models
{
    public class TimeDataModel
    {
        public DateTime ServerTime { get; private set; }
        public bool IsTimeLoaded { get; private set; }

        public TimeDataModel()
        {
            ServerTime = DateTime.Now;
            IsTimeLoaded = false;
        }

        public void SetServerTime(DateTime time)
        {
            ServerTime = time;
            IsTimeLoaded = true;
        }
    }
}
