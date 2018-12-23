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
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using CasingDesign.API.Entities;
using CasingDesign.API.Services;
using CasingDesign.API.Models;

namespace CasingDesign.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<CasingDesignContext>(options=>options.UseSqlite("Data Source=/Users/plopata/Desktop/AGH/inzynierka/CasingDesign.API/CasingInventory.db"));
            //services.AddDbContext<CasingDesignContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CasingInventory")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddScoped<ICasingDesignRepository, CasingDesignRepository>(); 
            services.AddScoped<ICalculateVerticalRepository, CalculateVerticalRepository>(); 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();

            if (env.IsDevelopment()) 
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
                
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            AutoMapper.Mapper.Initialize(cfg => 
            {
                cfg.CreateMap<Entities.Casing, Models.CasingDto>();
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
