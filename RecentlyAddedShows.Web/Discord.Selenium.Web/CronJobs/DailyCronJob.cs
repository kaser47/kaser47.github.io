using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class DailyCronJob : CronJobService
    {
        public DailyCronJob(IScheduleConfig<DailyCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.Daily();
            };
            return base.DoWork(cancellationToken);
        }
    }
}