using System;

namespace ClockApp.Models
{
    public class TimeModel
    {
        public DateTime CurrentTime { get; private set; }

        // Пустой конструктор для Zenject
        public TimeModel()
        {
            CurrentTime = DateTime.Now;
        }

        // Метод для установки начального времени
        public void SetInitialTime(DateTime time)
        {
            CurrentTime = time;
        }

        public void UpdateTime(DateTime newTime)
        {
            CurrentTime = newTime;
        }

        public void Tick(TimeSpan deltaTime)
        {
            CurrentTime = CurrentTime.Add(deltaTime);
        }
    }
}
