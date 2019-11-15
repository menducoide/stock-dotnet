using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using stock_dotnet.utils.db;
using stock_dotnet.stock;
using stock_dotnet.utils.Exceptions;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using stock_dotnet.utils.Extensions;
using stock_dotnet.utils;
using stock_dotnet.utils.env;
using stock_dotnet.security;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using stock_dotnet.utils.rabbitmq;
using stock_dotnet.utils.cache;

namespace stock_dotnet
{
    public class Startup
    {
        public Startup(ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            //    Configuration = configuration;
            var builder = new ConfigurationBuilder()
                        .SetBasePath(environment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

            Configuration = builder.Build();
         

        }

        public IConfiguration Configuration { get; }
   

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.Configure<MongoDatabaseSettings>(
              Configuration.GetSection("StockDatabaseSettings"));
            services.Configure<Env>(Configuration.GetSection("Configuration"));
            services.AddSingleton<IMongoDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<MongoDatabaseSettings>>().Value);
            services.AddSingleton<IEnv>(sp =>
      sp.GetRequiredService<IOptions<Env>>().Value);
            services.AddSingleton<StockRepository>();
            services.AddSingleton<StockService>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<ICacheHandler, CacheHandler>();
            services.AddSingleton<EmiterRabbit>();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddSingleton<Security>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //     services.AddAuthentication("ApiKeyAuth")
            //   .AddScheme<ApiKeyAuthOpts, ApiKeyAuthHandler>("ApiKeyAuth", "ApiKeyAuth", opts => { });
            services.AddTransient<IAuthorizationHandler, AuthenticationHandler>();
            services.AddAuthorization(authConfig =>
            {
                authConfig.AddPolicy("admin",
                    policyBuilder => policyBuilder
                        .AddRequirements(new Role("admin")));
                authConfig.AddPolicy("user-loged",
                    policyBuilder => policyBuilder
                        .AddRequirements(new Role(string.Empty)));
            });
            services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(
                            builder =>
                            {
                                builder
                                .AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                            });
                    });

            //    var provider = services.BuildServiceProvider();
            services.AddHostedService<ConsumeFanoutService>();
            services.AddHostedService<ConsumeArticleDeletedService>();
            // var rabbitService = provider.GetService(typeof(RabbitFanout)) as RabbitFanout;
            // rabbitService.Subscribe();
            // RabbitFanout.Subscribe();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.ConfigureCustomExceptionMiddleware();
            app.UseHttpsRedirection();
            app.UseMvc();
            //             app.UseMvc(routes =>
            // {
            //     routes.MapRoute("default", "{controller=Default}/{action=Index}/{id?}");
            // });
        }

    }


}
