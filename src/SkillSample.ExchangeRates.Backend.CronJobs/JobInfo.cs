namespace SkillSample.ExchangeRates.Backend.CronJobs
{
    public class JobInfo
    {
        public JobScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class JobScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
