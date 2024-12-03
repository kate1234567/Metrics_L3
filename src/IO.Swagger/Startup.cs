using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using IO.Swagger.Filters;
using IO.Swagger.Services;
using Prometheus;
using IO.Swagger.Middlewares;
using Serilog;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Exporter;

namespace IO.Swagger
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _hostingEnv = env;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration) 
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(); 
            });

            services.AddOpenTelemetry()
                .ConfigureResource(resources => resources.AddService("movie_library"))
                .WithTracing(tr => tr
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddZipkinExporter(o =>
                    {
                        o.Endpoint = new Uri("http://172.18.0.1:9411/api/v2/spans");
                    }));
           
            services
                .AddControllers()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                })
                .AddXmlSerializerFormatters();

            services.AddHealthChecks();
            services.AddSingleton<MetricsService>();

            // Swagger configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Movie Library",
                    Description = "Movie Library (ASP.NET Core 7.0)",
                    Contact = new OpenApiContact
                    {
                        Name = "Swagger Codegen Contributors",
                        Url = new Uri("https://github.com/swagger-api/swagger-codegen"),
                    },
                    TermsOfService = new Uri("https://example.com/terms"),
                });
                c.CustomSchemaIds(type => type.FullName);
                c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{_hostingEnv.ApplicationName}.xml");
                c.OperationFilter<GeneratePathParamsValidationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRouting();

            // Logging requests
            app.UseSerilogRequestLogging();

            // Authorization and authentication middleware
            app.UseAuthorization();
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.UseMiddleware<MetricsTracking>();

            // Swagger middleware to serve Swagger JSON and Swagger UI
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Library");
                c.RoutePrefix = "swagger";
            });

            // Define endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            // Error handling middleware
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
        }
    }
}
