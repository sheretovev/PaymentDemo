// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using System.Linq;
using Demo.Payment.Adyen;
using Demo.Payment.Stripe;
using Demo.WebApi.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Demo.WebApi.Logging;

namespace Demo.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // register application settings            
            services.AddSingleton(x => Configuration.GetSection(nameof(AdyenSettings)).Get<AdyenSettings>());
            services.AddSingleton(x => Configuration.GetSection(nameof(StripeSettings)).Get<StripeSettings>());

            //services.AddCors(options=>
            //{
            //    // this defines a CORS policy called "default"
            //    options.AddPolicy("default", policy =>
            //    {
            //        policy.WithOrigins(corsSettings.clientUrls.Select(x => x.TrimEnd('/')).ToArray())
            //            .AllowAnyHeader()
            //            .AllowAnyMethod()
            //            .AllowCredentials();
            //    });
            //});

            services.AddMvc();
            services.AddApiVersioning(
                    o =>
                    {
                        o.AssumeDefaultVersionWhenUnspecified = true;
                        o.DefaultApiVersion = new ApiVersion(1, 0);
                        o.ApiVersionReader = new HeaderApiVersionReader("api-version");
                    }
            );

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info() { Title = "Demo WebApi v1", Version = "v1" });
                options.SwaggerDoc("v1.1", new Info() { Title = "Demo WebApi v1.1", Version = "v1.1" });
                options.SwaggerDoc("v2", new Info() { Title = "Demo WebApi v2", Version = "v2" });
            }
            );

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
        }
    

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            SerilogConfigurator.Configure(env);
            loggerFactory.AddSerilog(Log.Logger);
            app.UseCors("default");

            //app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            //{
            //    Authority = corsSettings.authorityUrl,
            //    RequireHttpsMetadata = false,
            //    ApiName = "LeasingWebApi"
            //});

            app.UseMvc();
            
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo WebApi")
            );
        }
    }
}