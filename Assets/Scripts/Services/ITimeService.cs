using System;
using System.Collections;
using ClockApp.Models;

namespace ClockApp.Services
{
    public interface ITimeService
    {
        IEnumerator GetServerTime(Action<DateTime> onSuccess, Action<string> onError);
    }
}
