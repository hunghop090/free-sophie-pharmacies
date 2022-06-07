using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Core;
using App.Mapping;
using App.SharedLib;
using App.SharedLib.Seeds;
using Sophie.Services.NotificationService;
using Sophie.Services.EmailSenderService;
using Microsoft.Extensions.Localization;
using Sophie.Languages;
using Sophie.Repository;
using Sophie.Repository.Interface;
using EasyCronJob.Core;
using Amazon.Runtime;
using Amazon.S3;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using FluentValidation.AspNetCore;
using App.Core.Units;
using System.Runtime.InteropServices;

namespace Sophie
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
            services.AddMvc();
            services.AddRazorPages();

            MongoExtensions.ConfigureServices(services, this.Configuration);
            SecurityExtensions.AddSecurity(services, this.Configuration);
            SharedLibExtensions.ConfigureServices(services, this.Configuration);

            // AutoMapper.Extensions.Microsoft.DependencyInjection
            services.AddAutoMapper(typeof(Startup));
            MapperConfiguration mappingConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
            .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<Startup>());

            //services.AddRouting(options => options.LowercaseUrls = true);

            // AWS Configuration from AWSSDK.Core + AWSSDK.Extensions.NETCore.Setup + AWSSDK.S3
            services.Configure<Sophie.Model.AWSOptions>(this.Configuration.GetSection("AWSOptions")); // Inject Data
            Amazon.Extensions.NETCore.Setup.AWSOptions awsOption = this.Configuration.GetAWSOptions("AWSOptions");
            awsOption.Region = Amazon.RegionEndpoint.APSoutheast1;
            awsOption.Credentials = new BasicAWSCredentials(this.Configuration["AWSOptions:AccessKey"], this.Configuration["AWSOptions:SecretKey"]);
            services.AddDefaultAWSOptions(awsOption);
            services.AddAWSService<IAmazonS3>();

            // Add more service
            services.AddSingleton<IStringLocalizer, JsonStringLocalizer>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IConfigurationsRepository, ConfigurationsRepository>();

            services.AddScoped<ILoginProviderRepository, LoginProviderRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IHospitalRepository, HospitalRepository>();
            services.AddScoped<IMedicalAppointmentRepository, MedicalAppointmentRepository>();
            services.AddScoped<IRelationshipRepository, RelationshipRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IInforRepository, InforRepository>();
            services.AddScoped<IHealthRepository, HealthRepository>();

            services.AddScoped<IPharmacistRepository, PharmacistRepository>();
            services.AddScoped<IAnalysisRepository, AnalysisRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IShopRepository, ShopRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<ITransportPromotionRepository, TransportPromotionRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IDraftOrderRepository, DraftOrderRepository>();
            services.AddControllersWithViews();
            // CronJobService https://github.com/furkandeveloper/EasyCronJob
            // https://crontab.guru
            services.ApplyResulation<ConsoleCronJob>(options =>
            {
                options.CronExpression = "0 21 * * *"; // minute hour dayInMonth month dayInWeek = "0 21 * * *" = "0 0-23 * * *"
                if (UntilsEx.GetSystemType() == OSPlatform.OSX || UntilsEx.GetSystemType() == OSPlatform.Linux)
                    options.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                else
                    options.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedUsersAndRoles seedUsersAndRoles)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            MongoExtensions.Configure(app, env, this.Configuration);
            SecurityExtensions.AddSecurity(app, this.Configuration);
            SharedLibExtensions.Configure(app, this.Configuration, seedUsersAndRoles);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "NotFound", pattern: "notfound/{code?}", defaults: new { controller = "Home", action = "Error" });
                endpoints.MapControllerRoute(name: "Default", pattern: "{controller=Home}/{action=Login}/{id?}", defaults: new { controller = "Home", action = "Login" });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "Default", template: "{controller=Home}/{action=Login}/{id?}");
            });
        }

    }
}
