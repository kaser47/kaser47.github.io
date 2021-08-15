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
}