using System;
using System.Timers;

namespace Codenames.Client.Services
{
    public class BlazorTimer
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