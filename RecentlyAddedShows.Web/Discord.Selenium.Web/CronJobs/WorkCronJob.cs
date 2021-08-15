using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Selenium.Web.CronJobs
{
    public class WorkCronJob : CronJobService
    {
        private readonly AutomatedDiscord automatedDiscord;

        public WorkCronJob(IScheduleConfig<WorkCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
            automatedDiscord = new AutomatedDiscord(Consts.webDriverLocation());
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            automatedDiscord.Work();
            automatedDiscord.Dispose();
            return base.DoWork(cancellationToken);
        }
    }
}
