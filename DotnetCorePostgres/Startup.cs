using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetCorePostgres.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace DotnetCorePostgres {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.Configure<CookiePolicyOptions> (options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Add PostgreSQL support
            services.AddEntityFrameworkNpgsql ()
                .AddDbContext<DockerCommandsDbContext> (options =>
                    options.UseNpgsql (Configuration["Data:DbContext:DockerCommandsConnectionString"]))
                .AddDbContext<CustomersDbContext> (options =>
                    options.UseNpgsql (Configuration["Data:DbContext:CustomersConnectionString"]));

            services.AddMvc ();
            // Add our PostgreSQL Repositories (scoped to each request)
            services.AddScoped<IDockerCommandsRepository, DockerCommandsRepository> ();
            services.AddScoped<ICustomersRepository, CustomersRepository> ();

            //Transient: Created each time they're needed
            services.AddTransient<DockerCommandsDbSeeder> ();
            services.AddTransient<CustomersDbSeeder> ();

            services.AddSwaggerGen (options => {
                options.SwaggerDoc ("v1", new Info {
                    Version = "v1",
                        Title = "Application API",
                        Description = "Application Documentation",
                        TermsOfService = "None",
                        Contact = new Contact { Name = "Author", Url = "" },
                        License = new License { Name = "MIT", Url = "https://en.wikipedia.org/wiki/MIT_License" }
                });

                // Add XML comment document by uncommenting the following
                // var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "MyApi.xml");
                // options.IncludeXmlComments(filePath);

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, DockerCommandsDbSeeder dockerCommandsDbSeeder, CustomersDbSeeder customersDbSeeder) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler ("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseStaticFiles ();
            app.UseCookiePolicy ();

            app.UseSwagger ();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            // Visit http://localhost:5000/swagger
            app.UseSwaggerUI (c => {
                c.SwaggerEndpoint ("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc ();

            customersDbSeeder.SeedAsync (app.ApplicationServices).Wait ();
            dockerCommandsDbSeeder.SeedAsync (app.ApplicationServices).Wait ();

        }
    }
}