using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetExtensions.RelayApiEntities;
using NetExtensions.Services;

namespace NetExtensions
{
    public static class RelayApiBuilderExtension
    {
        public static void ConfigureRelayApi<HTTPClient, TConfigureService, TExecuteService>(this IServiceCollection services, string baseAddress, string swaggerTitle = null,
            string swaggerDescription = null,
            string swaggerVersion = null) where HTTPClient : class where TConfigureService : ConfigureService where TExecuteService : ExecuteService
        {
            services.AddHttpClient<HTTPClient>(client => client.BaseAddress = new Uri(baseAddress));
            services.AddControllers().AddNewtonsoftJson();
            services.AddRelayApi(swaggerTitle, swaggerDescription, swaggerVersion);
            services.AddScoped<TConfigureService>();
            services.AddScoped<TExecuteService>();
        }

        public static IServiceCollection AddRelayApi(this IServiceCollection services, string swaggerTitle = null,
            string swaggerDescription = null,
            string swaggerVersion = null)
        {
            services.AddSwashbuckle(swaggerTitle, swaggerDescription, swaggerVersion);
            return services.AddSqliteInMemoryDb<RelayApiContext>();
        }

        public static IApplicationBuilder ConfigureRelayApi(this IApplicationBuilder app, IWebHostEnvironment env, string swaggerName = null,
            string swaggerEndpoint = null, bool useSerilogMiddleware = true)
        {
            app.UseRelayApi(swaggerName, swaggerEndpoint, useSerilogMiddleware);
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            return app;
        }

        public static IApplicationBuilder UseRelayApi(this IApplicationBuilder app, string swaggerName = null,
            string swaggerEndpoint = null, bool useSerilogMiddleware = true)
        {
            app.AddSwashbuckle(swaggerName, swaggerEndpoint);
            return app.AddSerilogRequestLogging(useSerilogMiddleware);
        }
    }
}