using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class SellSauceCronJob : CronJobService
    {
        public SellSauceCronJob(IScheduleConfig<SellSauceCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.SellSauce();
            };
            return base.DoWork(cancellationToken);
        }
    }
}