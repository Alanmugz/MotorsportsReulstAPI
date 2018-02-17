using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using NLog.Web;


namespace motorsportresultapi_public
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
            services.AddMvc()
                .AddApplicationPart(typeof(MotorsportResultAPI.Domain.Controllers.AutoCross.CompetitorController).GetTypeInfo().Assembly)
                .AddControllersAsServices();

            //https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2
            services.AddSingleton<NLog.ILogger>(NLogBuilder.ConfigureNLog("nlog.config").GetLogger("defaultLogger"));
            
            services.AddSingleton<MotorsportResultAPI.Data.AutoCross.ICompetitorRepository, MotorsportResultAPI.Data.AutoCross.CompetitorRepository>();
            services.AddTransient<MotorsportResultAPI.Data.AutoCross.ICompetitorRepository>(
                competitorRepository => new MotorsportResultAPI.Data.AutoCross.CompetitorRepository(
                    "MyConnectionString",
                    new MotorsportResultAPI.Data.AutoCross.Mapper(),
                    new MotorsportResultAPI.Data.Helper.Transformer()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
