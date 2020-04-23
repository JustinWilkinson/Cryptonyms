using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Jobs
{
    public class JobScheduler
    {
        private static readonly StdSchedulerFactory _schedulerFactory = new StdSchedulerFactory();
        private readonly IScheduler _scheduler;

        private JobScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public static async Task<JobScheduler> GetSchedulerAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();
            return new JobScheduler(scheduler);
        }

        public async Task ScheduleDailyJobAsync<T>(string jobName, string triggerName, int timeHours = 0, int timeMinutes = 0)
        {
            var job = JobBuilder.Create<CleanUpJob>().WithIdentity(jobName).Build();
            var trigger = TriggerBuilder.Create().WithIdentity(triggerName).ForJob(jobName).WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(timeHours, timeMinutes)).Build();
            await _scheduler.ScheduleJob(job, trigger);
        }
    }
}