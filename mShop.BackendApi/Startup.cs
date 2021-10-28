using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using mShop.Application.Catalog.Products;
using mShop.Application.Common;
using mShop.Application.System.Users;
using mShop.Data.EF;
using mShop.Data.Entities;
using mShop.Ultilities.Constants;

namespace mShop.BackendApi
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
            // 1. cau hinh chuoi ket noi
            //services.AddDbContext<MShopDbContext>(option =>
            //option.UseSqlServer(Configuration.GetConnectionString("mShopDb")));
            services.AddDbContext<MShopDbContext>(option =>
            option.UseSqlServer(Configuration.GetConnectionString(SystemConstants.MainConnectionString)));

            // 2. DI cac service

            // khai bao tat ca cho UsersController.cs
            // khai bao nay de applly all service cua AppUser, AppRole
            services.AddIdentity<AppUser, AppRole>()
                    .AddEntityFrameworkStores<MShopDbContext>()
                    .AddDefaultTokenProviders();

            // Phai khai bao cac service se dung trong cac controller
            // Moi lan yc 1 obj IPublicProductService thi se khoi tao 1 obj class PublicProductService
            services.AddTransient<IPublicProductService, PublicProductService>();
            services.AddTransient<IManageProductService, ManagerProductService>();
            services.AddTransient<IStorageService, FileStorageService>();           // IStorageServie khai bao trong ManageService nen fai khai bao khi dung

            services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
            services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();
            services.AddTransient<IUserService, UserService>();

            // 3. mac dinh
            services.AddControllersWithViews();

            // 4. Add swager
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            //
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

            // UserSwaggerUI
            // Duong dan chua file mo to cac function trong swagger
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger eShop V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}