using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class WorkCronJob : CronJobService
    {
        public WorkCronJob(IScheduleConfig<WorkCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.Work();
            };
            return base.DoWork(cancellationToken);
        }
    }
}
