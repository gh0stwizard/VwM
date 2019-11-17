using System;
using NCrontab;

namespace VwM.BackgroundServices.Cron
{
    class CronTaskWrapper
    {
        public CrontabSchedule Schedule { get; set; }
        public ICronTask Task { get; set; }

        public DateTime LastRunTime { get; set; }
        public DateTime NextRunTime { get; set; }

        public void Increment()
        {
            LastRunTime = NextRunTime;
            NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
        }

        public bool ShouldRun(DateTime currentTime)
        {
            return NextRunTime < currentTime && LastRunTime != NextRunTime;
        }
    }
}
