using System.Threading;
using System.Threading.Tasks;

namespace Discord.Selenium.Web.CronJobs
{
    public class TipsCronJob : CronJobService
    {
        private readonly AutomatedDiscord automatedDiscord;

        public TipsCronJob(IScheduleConfig<TipsCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            automatedDiscord = new AutomatedDiscord(Consts.webDriverLocation());
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            automatedDiscord.Tips();
            automatedDiscord.Dispose();
            return base.DoWork(cancellationToken);
        }
    }
}