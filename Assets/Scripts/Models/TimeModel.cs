using System;
using UnityEngine;

namespace ClockApp.Models
{
    public class TimeModel
    {
        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get => _currentTime;
            set => _currentTime = value;
        }

        public TimeModel()
        {
            _currentTime = DateTime.Now;
        }

        public void UpdateTime(DateTime newTime)
        {
            _currentTime = newTime;
        }

        public void Tick(TimeSpan deltaTime)
        {
            _currentTime = _currentTime.Add(deltaTime);
        }
    }
}
