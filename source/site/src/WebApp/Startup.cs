namespace MyHomework.WebApp
{
    using Basic;
    using Configurations;
    using DatabaseModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Middlewares;
    using WeChat;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if(env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Application Insights data collection services to the services container.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddOptions();
            services.Configure<AppOptions>(Configuration.GetSection("AppOptions"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.AddSingleton(typeof(WeChatApi));
            services.AddSingleton(typeof(BlobStorageManager));

            // Add framework services.
            services.AddMvc();
            
            var sp = services.BuildServiceProvider();
            var appOptions = sp.GetService<IOptions<AppOptions>>();
            var connectionStrings = sp.GetService<IOptions<ConnectionStrings>>();
            services.UseCustomAuthorization(appOptions);

            services.AddDbContext<MyHomeworkDBContext>(options => options.UseSqlServer(connectionStrings.Value.MyHomeworkDatabase));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var options = app.ApplicationServices.GetService<IOptions<AppOptions>>();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Add Application Insights to the request pipeline to track HTTP request telemetry data.
            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Track data about exceptions from the application. Should be configured after all error handling middleware in the request pipeline.
            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMiddleware<HttpsRedirectionMiddleware>();

            app.UseCustomAuthentication(options);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Welcome}/{id?}");
            });
        }
    }
}
