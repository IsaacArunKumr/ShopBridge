using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopBridge.Models;
using Microsoft.OData.Edm;
using ShopBridge.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Builder;
using System.Linq;

namespace ShopBridge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services)
        {
            services.AddDbContext<DataContext>(opt=>opt.UseSqlServer(Configuration.GetConnectionString("ShopBridgeContext")));
            services.AddMvc(opt=>opt.EnableEndpointRouting=false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers(opt=>opt.Filters.Add(new HttpResponseExceptionFilter()));
            services.AddOData();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllers();
                endpoints.Select().Filter().OrderBy().Count().Expand().MaxTop(100);
                endpoints.MapODataRoute("ODataRoute", "odata", GetEdmModel());
            });

            #region seed data
            using (var context = new DataContext())
            {
                context.Database.EnsureCreated();

                if (!context.Products.Any())
                {
                    context.Products.AddRange(new[] {
                            new Product { Id = 1, Name = "Olive Oil", Description = " Olive Oil", CategoryId = 3 },
                            new Product { Id = 2, Name = "Sunflower Oil", Description = " Sunflower Oil", CategoryId = 3 },
                            new Product { Id = 3, Name = "RiceBran Oil", Description = " RiceBran Oil", CategoryId = 3 },
                            new Product { Id = 4, Name = "Pepsi", Description = " Pepsi", CategoryId = 1 },
                            new Product { Id = 5, Name = "Bovanto", Description = " Bovanto", CategoryId = 1 },
                            new Product { Id = 6, Name = "Coke", Description = " Coke", CategoryId = 1 },
                            new Product { Id = 7, Name = "Surf Excel", Description = " Surf Excel", CategoryId = 2 },
                            new Product { Id = 8, Name = "Arial", Description = " Arial", CategoryId = 2 },
                            new Product { Id = 9, Name = "Comfort", Description = " Comfort", CategoryId = 2 }
                    });
                }
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new[] {
                            new Category { CategoryId = 1, CategoryName = "Beverages", Description = "Beverage" },
                            new Category { CategoryId = 2, CategoryName = "Laundry", Description = "Laundry Products" },
                            new Category { CategoryId = 3, CategoryName = "Oil", Description = "Edible Oil" }
                    });
                }
                context.SaveChanges();
            }
            #endregion
        }
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Category>("Categories");
            return builder.GetEdmModel();
        }
    }
}
