using System.Threading;
using System.Threading.Tasks;

namespace Discord.Selenium.Web.CronJobs
{
    public class CleanCronJob : CronJobService
    {
        private readonly AutomatedDiscord automatedDiscord;

        public CleanCronJob(IScheduleConfig<CleanCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            automatedDiscord = new AutomatedDiscord(Consts.webDriverLocation());
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            automatedDiscord.Clean();
            automatedDiscord.Dispose();
            return base.DoWork(cancellationToken);
        }
    }

    public class ClaimCronJob : CronJobService
    {
        private readonly AutomatedDiscord automatedDiscord;

        public ClaimCronJob(IScheduleConfig<ClaimCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            automatedDiscord = new AutomatedDiscord(Consts.webDriverLocation());
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            automatedDiscord.Claim();
            automatedDiscord.Dispose();
            return base.DoWork(cancellationToken);
        }
    }

    public class DailyCronJob : CronJobService
    {
        private readonly AutomatedDiscord automatedDiscord;

        public DailyCronJob(IScheduleConfig<DailyCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            automatedDiscord = new AutomatedDiscord(Consts.webDriverLocation());
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            automatedDiscord.Daily();
            automatedDiscord.Dispose();
            return base.DoWork(cancellationToken);
        }
    }
}