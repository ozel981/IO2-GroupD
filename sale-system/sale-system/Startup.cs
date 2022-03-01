using SaleSystem.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SaleSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaleSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Cors;
using FluentValidation.AspNetCore;
using Microsoft.Data.Sqlite;
using SaleSystem.Wrappers;

namespace SaleSystem
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
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SaleSystem", Version = "v1" });
            });

            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<INewsletterService, NewsletterService>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddSingleton<SendGridWrapper>();

            var dbConn = GetDbConnConfig();
            var dbConnString = Configuration.GetConnectionString(dbConn);
            if (dbConn == "SqliteConnection")
            {
                var connection = new SqliteConnection(dbConnString);
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "PRAGMA journal_mode=WAL;";
                command.ExecuteNonQuery();


                services.AddDbContext<SaleSystemDBContext>(options =>
                    options.UseSqlite(connection));

            }
            else
            {
                services.AddDbContext<SaleSystemDBContext>(options =>
                    options.UseSqlServer(dbConnString), ServiceLifetime.Scoped);
            }


            // Add AllowAll policy just like in single controller example.
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                   builder =>
                   {
                       builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
                   });
            });

            // Add framework services.
            services.AddMvc();
        }
        private static string GetDbConnConfig()
        {
            var config = Environment.GetEnvironmentVariable("DB_CONNECTION");
            if (config is null)
            {
                return "DefaultConnection";
            }
            return config;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "sale_system v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
