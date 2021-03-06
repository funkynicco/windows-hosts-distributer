﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using WHD.IoC;
using Microsoft.AspNetCore.StaticFiles;

namespace WHD
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Console.Title = $"{env.ApplicationName} - Kestrel";

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("connectionstrings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(_configuration);

            services.AddMvc();
            services.AddMemoryCache();

            ApiConfiguration.Configure(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();

            // add support for woff
            var provider = new FileExtensionContentTypeProvider();
            var staticFileOptions = new StaticFileOptions() { ContentTypeProvider = provider };
            //provider.Mappings.Add(".woff2", "application/font-woff2");
            if (env.IsDevelopment())
            {
                staticFileOptions.OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                    context.Context.Response.Headers["Pragma"] = "no-cache";
                    context.Context.Response.Headers["Expires"] = "-1";
                };
            }
            app.UseStaticFiles(staticFileOptions);

            app.UseMvc();
        }
    }
}
