using System.Threading;
using System.Threading.Tasks;

namespace Discord.Selenium.Web.CronJobs
{
    public class OvertimeCronJob : CronJobService
    {
        private readonly AutomatedDiscord automatedDiscord;

        public OvertimeCronJob(IScheduleConfig<OvertimeCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            automatedDiscord = new AutomatedDiscord(Consts.webDriverLocation());
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            automatedDiscord.Overtime();
            automatedDiscord.Dispose();
            return base.DoWork(cancellationToken);
        }
    }
}