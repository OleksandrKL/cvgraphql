﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Http;
using GraphQL.Types;
using GraphQL;
using WebAppGraphQL.Middleware;
using WebAppGraphQL.GraphQL;
using Model;
using Repository;


namespace WebAppGraphQL
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            //CVConnection cvConnection = new CVConnection {
            //    ConnectionString= @"data source=(LocalDb)\MSSQLLocalDB; initial catalog=CV; integrated security=True;"
            //};

            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddSingleton(new CVContext());

            services.AddSingleton<PersonType>();
            services.AddSingleton<PersonInputType>();
            services.AddSingleton<SkillType>();
            services.AddSingleton<CompanyType>();
            services.AddSingleton<ProjectType>();
            services.AddSingleton<EducationType>();
            services.AddSingleton<PersonQuery>();
            services.AddSingleton<PersonMutation>();
            services.AddSingleton<PositionType>();
            services.AddSingleton<DurationType>();
            services.AddSingleton<PersonSkillType>();
            services.AddSingleton<ISchema>(s => new PersonSchema(type => (GraphType)s.GetService(type)));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var d = app.ApplicationServices.GetService<ISchema>();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseGraphQLEndpoint();
            app.UseMvc();

        }
    }

}
