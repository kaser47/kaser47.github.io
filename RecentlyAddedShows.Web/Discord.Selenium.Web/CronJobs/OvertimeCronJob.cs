using System.Threading;
using System.Threading.Tasks;
using Discord.Selenium.Web.Data;

namespace Discord.Selenium.Web.CronJobs
{
    public class OvertimeCronJob : CronJobService
    {
        public OvertimeCronJob(IScheduleConfig<OvertimeCronJob> config) : base(config.CronExpression, config.TimeZoneInfo)
        {
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var discord = new AutomatedDiscord())
            {
                discord.Overtime();
            };
            return base.DoWork(cancellationToken);
        }
    }
}