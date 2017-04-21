using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIService.Extensions;
using APIService.Handlers;
using APIService.Repository;
using APIService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Rabbit;
using Steeltoe.CloudFoundry.Connector.Redis;
using Steeltoe.Extensions.Configuration;

namespace APIService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILoggerFactory logFactory)
        {
            logFactory.AddConsole(minLevel: LogLevel.Debug);

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCloudFoundry();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             // Add framework services.
            services.AddMvc();
            
            services.AddRedisConnectionMultiplexer(Configuration);
            services.AddRabbitConnection(Configuration);

            services.AddSwaggerGen();

            services.ConfigureSwaggerGen(options =>
            {
                //options.AddSecurityDefinition("IdentityServer", sch);
                //options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                options.DescribeAllEnumsAsStrings();
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info()
                {
                    Title = "Example API Service HTTP API",
                    Version = "v1",
                    Description = "The API Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddTransient<IMetricsRepository, RedisMetricsRepository>();
            services.AddTransient<IMessageHandler, LogMessageHandler>();
            services.AddSingleton<IQueueConsumerService, QueueConsumerService>();
            services.AddSingleton<IQueueService, QueueService>();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.StartQueueService();
            
            app.UseCors("CorsPolicy");

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
                .UseSwaggerUi();
        }
    }
}
