using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Selenium.Web.CronJobs;

namespace Discord.Selenium.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCronJob<WorkCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/10 * * * *";
            });

            services.AddCronJob<TipsCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/5 * * * *";
            });

            services.AddCronJob<OvertimeCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/30 * * * *";
            });

            services.AddCronJob<CleanCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 8 * * *";
            });

            //services.AddCronJob<WorkCronJob>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"*/1 * * * *";
            //});

            //services.AddCronJob<TipsCronJob>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"*/1 * * * *";
            //});

            //services.AddCronJob<OvertimeCronJob>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"*/1 * * * *";
            //});

            //services.AddCronJob<CleanCronJob>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"*/1 * * * *";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
