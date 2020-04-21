using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CW3.DAL;
using CW3.Handlers;
using CW3.Middleware;
using CW3.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CW3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        

        // This method gets called by the runtime. Use this method to add services to the tcontainer.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IStudentsDbService, SqlServerDbService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentsDbService service)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMiddleware<LoggingMiddleware>();
           
            
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("You don't give index");
                    return;
                }
                var bodyStream = string.Empty;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStream = await reader.ReadToEndAsync();
                }



                string index = context.Request.Headers["Index"].ToString();
                
                if (!service.CheckIfExists(index))//connection with database
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("doesn't exist");
                    return;
                }
                await next();
            });

           
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}