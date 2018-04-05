using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Threading;
using WebAppP2P.WebSockets.InternalMessages;
using WebAppP2P.WebSockets;
using FluentScheduler;
using WebAppP2P.Core;
using WebAppP2P.Core.Keys;
using WebAppP2P.Core.Messages;
using WebAppP2P.Services;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAppP2P.Core.Database;
using Microsoft.Extensions.Options;
using WebAppP2P.Jobs;
using WebAppP2P.Core.Nodes;

namespace WebAppP2P
{
    public class Startup
    {
        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("./nodeOptions.json", false, true)
                .Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddInternalMessagesServices();
            services.AddCoreServices();

            services.AddOptions();
            services.Configure<NodeOptions>(Configuration);
            services.AddAutoMapper();
            services.AddSingleton<AppRegistry>();
            services.AddTransient<TracerJob>();
            services.AddTransient<BlockchainJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ConfigureApplication(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new Microsoft.AspNetCore.SpaServices.Webpack.WebpackDevMiddlewareOptions()
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            app.UseCors(policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });
            app.UseStaticFiles();
            app.UseDefaultFiles();
           
            app.UseWebSockets();
            app.UseMiddleware<WebSocketMiddleware>("^/{1,2}messages$");

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("api", "api/{controller}/{action?}/{id?}");
            });
            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new { controller = "Home", action = "Index" });
                });
            });

            loggerFactory.AddConsole(LogLevel.Error);
        }

        private void ConfigureApplication(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<ApplicationDatabase>().Database;
                db.Migrate();

                var dbInitializer = serviceScope.ServiceProvider.GetService<DatabaseInitializer>();
                dbInitializer.Seed();

                var nodeOptions = serviceScope.ServiceProvider.GetService<IOptionsSnapshot<NodeOptions>>();

                if (string.IsNullOrEmpty(nodeOptions.Value.Self))
                {
                    throw new ArgumentNullException("Configuration option not found: " + nameof(nodeOptions.Value.Self));
                }

                var nodeListWithSelf = nodeOptions.Value.NodesList.ToList();
                nodeListWithSelf.Add(nodeOptions.Value.Self);
                foreach (var node in nodeListWithSelf.Where(n => !n.EndsWith("/")))
                {
                    throw new ArgumentException(string.Format("Incorrect url: \"{0}\". Urls in nodeOptions must end with /.", node));
                }

                var nodesRepository = serviceScope.ServiceProvider.GetService<INodesRepository>();
                nodesRepository.AddNodesAsync(nodeListWithSelf);

                JobManager.Initialize(serviceScope.ServiceProvider.GetService<AppRegistry>());
            }
        }
    }
}
