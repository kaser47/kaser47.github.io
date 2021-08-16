using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class CleanCronJob : CronJobService
    {
        public CleanCronJob(IScheduleConfig<CleanCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {

        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.Clean();
            };
            return base.DoWork(cancellationToken);
        }
    }
}