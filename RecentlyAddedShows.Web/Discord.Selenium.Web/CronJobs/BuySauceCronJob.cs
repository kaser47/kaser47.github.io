using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class BuySauceCronJob : CronJobService
    {
        public BuySauceCronJob(IScheduleConfig<BuySauceCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {

        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.BuySauce();
            };
            return base.DoWork(cancellationToken);
        }
    }
}