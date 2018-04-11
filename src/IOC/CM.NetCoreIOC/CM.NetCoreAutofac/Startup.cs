using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc;
using CM.NetCoreAutofac.Controllers;
namespace CM.NetCoreAutofac
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            services.AddMvc();
            var builder = new ContainerBuilder();
            builder.RegisterType(typeof(Dog)).As(typeof(Animal))
                        .InstancePerLifetimeScope()
                        .PropertiesAutowired();
            builder.RegisterType(typeof(TestB)).As(typeof(TestA))
                        .InstancePerLifetimeScope()
                        .PropertiesAutowired();
            builder.RegisterType(typeof(HomeController))//.As(typeof(ControllerBase))
                        .InstancePerLifetimeScope()
                        .PropertiesAutowired();
            builder.Populate(services);
            return new AutofacServiceProvider(builder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            var animal1 = ActivatorUtilities.GetServiceOrCreateInstance(app.ApplicationServices, typeof(Animal));
            var animal2 = app.ApplicationServices.GetService(typeof(Animal));            
        }
    }
}
