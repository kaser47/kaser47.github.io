using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class TipsCronJob : CronJobService
    {
        public TipsCronJob(IScheduleConfig<TipsCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.Tips();
            };
            return base.DoWork(cancellationToken);
        }
    }
}