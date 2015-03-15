namespace CleanLiving.Engine
{
    public class ThreadedSpinWaitSchedulerOptions
    {
        public string SchedulerThreadName { get; set; }
        public long AcceptableSpinWaitPeriodNanoseconds { get; set; }
        public int SpinWaitIterations { get; set; }
    }
}
