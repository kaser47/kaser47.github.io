using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class ClaimCronJob : CronJobService
    {
        public ClaimCronJob(IScheduleConfig<ClaimCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {

        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.Claim();
            };
            return base.DoWork(cancellationToken);
        }
    }
}