using System;
using System.Timers;

namespace Cryptonyms.Client.Services
{
    public interface IBlazorTimer
    {
        public event Action OnElapsed;

        public void SetTimer(double interval);

        public void StopTimer();
    }

    public class BlazorTimer : IBlazorTimer
    {
        private Timer _timer;

        public event Action OnElapsed;

        public void SetTimer(double interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.Enabled = true;
        }

        public void StopTimer() => _timer?.Stop();

        private void NotifyTimerElapsed(object source, ElapsedEventArgs e)
        {
            OnElapsed?.Invoke();
            _timer.Dispose();
        }
    }
}