using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Model.Appsetting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookkeeping
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
            //�C��Call Method���`�J�@�ӷs��
            //services.AddTransient

            //�C��LifeCycle�`�J�@�ӷs��
            //services.AddScoped   

            //�u�|�b���x�Ұʮɪ`�J�@�ӷs��
            //services.AddSingleton

            services.AddTransient<ILineBotMessageService, LineBotMessageService>();
            services.AddTransient<IGoogleSheetService, GoogleSheetService>();

            services.AddControllers();
            services.Configure<LineBot>(Configuration.GetSection("LineBot"));
            services.Configure<GoogleSheetCredential>(Configuration.GetSection("GoogleSheetCredential"));
            services.Configure<GoogleSheetModel>(Configuration.GetSection("GoogleSheet"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bookkeeping", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookkeeping v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
