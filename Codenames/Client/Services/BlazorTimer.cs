using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
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

        private void NotifyTimerElapsed(object source, ElapsedEventArgs e)
        {
            OnElapsed?.Invoke();
            _timer.Dispose();
        }
    }

    public static class BlazorTimerExtensions
    {
        public static void AddBlazorTimer(this IServiceCollection services) => services.AddTransient<BlazorTimer>();
    }
}