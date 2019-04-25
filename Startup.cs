using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.Extensions.FileProviders;
using System.IO;

using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using ServiceStack.DataAnnotations;

using JustinBB.Models;

namespace JustinBB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var dbFactory = new OrmLiteConnectionFactory(configuration.GetConnectionString("Main"), MySqlDialect.Provider);
            Program.DB = dbFactory.Open();

            // Uncomment to clear tables every restart.
            // Program.DB.DropTable<Vote>();
            // Program.DB.DropTable<Post>();
            // Program.DB.DropTable<Topic>();
            // Program.DB.DropTable<Pokemon>();
            // Program.DB.DropTable<User>();

            Program.DB.CreateTable<User>();
            Program.DB.CreateTable<Pokemon>();
            Program.DB.CreateTable<Topic>();
            Program.DB.CreateTable<Post>();
            Program.DB.CreateTable<Vote>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            // Configure session information.
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromDays(30);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                // options.Cookie.IsEssential = true;
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Add another directory for static files. Set the flag to true,
            // otherwise only files with extensions will be accessible.
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
                RequestPath = "/uploads",

                ServeUnknownFileTypes = true
            });

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class DBString
    {

    }
}
