using System;
using System.Collections.Generic;
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
                .AddApplicationPart(typeof(MotorsportResultAPI.Domain.Controllers.Rally.CompetitorController).GetTypeInfo().Assembly)
                .AddControllersAsServices();

            //https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetLogger("defaultLogger");
            services.AddSingleton<NLog.ILogger>(logger);

            var _mapper = new MotorsportResultAPI.Data.Rally.Mapper();
            var _transformer = new MotorsportResultAPI.Data.Helper.Transformer();

            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            var _competitoryRepository = new MotorsportResultAPI.Data.Rally.CompetitorRepository(
                    logger,
                    Configuration.GetSection("ConnectionStrings")["Database"],
                    _mapper,
                    _transformer);

            services.AddSingleton<MotorsportResultAPI.Data.Rally.ICompetitorRepository, MotorsportResultAPI.Data.Rally.CompetitorRepository>();

            services.AddTransient<MotorsportResultAPI.Domain.Workflow.Rally.IGetCompetitor>(getCompetitor =>
                new MotorsportResultAPI.Domain.Workflow.Rally.GetCompetitor(_competitoryRepository, _mapper));
            services.AddTransient<MotorsportResultAPI.Domain.Workflow.Rally.IGetEvent>(getEvent =>
                new MotorsportResultAPI.Domain.Workflow.Rally.GetEvent(_competitoryRepository, _mapper));
            services.AddTransient<MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitor>(postCompetitor =>
                new MotorsportResultAPI.Domain.Workflow.Rally.PostCompetitor(_competitoryRepository, _mapper));
            services.AddTransient<MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitorAppend>(postCompetitorAppend =>
                new MotorsportResultAPI.Domain.Workflow.Rally.PostCompetitorAppend(_competitoryRepository, _mapper, _transformer));
            services.AddTransient<MotorsportResultAPI.Domain.Workflow.Rally.IPutCompetitorUpdate>(putCompetitorUpdate =>
                new MotorsportResultAPI.Domain.Workflow.Rally.PutCompetitorUpdate(_competitoryRepository, _mapper, _transformer));

            services.AddTransient<MotorsportResultAPI.Data.Rally.ICompetitorRepository>(
                competitorRepository => _competitoryRepository);
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
