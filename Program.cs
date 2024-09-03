using Rbac_IctJohor;
using Rbac_IctJohor.common;
using Rbac_IctJohor.Extensions;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Repositories;
using Rbac_IctJohor.Services;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Npgsql;
using System.Data;
using System.Net.Http;
using Microsoft.Extensions.Options;

internal class Program
{
    //private static async Task Main(string[] args)
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        string _appPolicy = "RbacIctJohor";
        // Add services to the container.
        var appSettings = new AppSettings();
        builder.Configuration.Bind(appSettings);

        // Configure CORS 
        builder.Services.AddCors(
            options => options.AddPolicy(
                _appPolicy,
                builder => builder
                    .WithOrigins(
                        appSettings.CORS
                     )
                     .SetIsOriginAllowed(isOriginAllowed: _ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )
        );

        builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(appSettings.ConnectionStrings.AppConnection));

        builder.Services.AddIdentity<User, Role>(c =>
        {
            c.Password.RequiredLength = 6;
            c.Password.RequireDigit = true;
            c.Password.RequireNonAlphanumeric = true;
            c.Password.RequireUppercase = true;
            c.Password.RequireLowercase = true;

            // Allow Custom Specified Characters in UserName
            c.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";

        }).AddEntityFrameworkStores<AppDbContext>()
        .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

        builder.Services.AddJWTAuthentication(appSettings);
        builder.Services.AddControllersWithViews()
             .AddNewtonsoftJson(options =>
             {
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                 options.SerializerSettings.Converters.Add(new StringEnumConverter());
             });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "UCM RBAC ICT JOHOR", Version = "v1", });

        });

        builder.Services.AddSwaggerGenNewtonsoftSupport();
        builder.Services.AddJWTSwaggerAuth();
        builder.Services.RegisterTokenConfigToDI(appSettings);
        builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        //interface service
        //builder.Services.AddTransient<BaseService>();
        //builder.Services.AddTransient<ICategoryService, CategoryService>();
        //builder.Services.AddTransient<ISubCategoryService, SubCategoryService>();
        //builder.Services.AddTransient<IAssetTypeService, AssetTypeService>();
        //builder.Services.AddTransient<IAssetService, AssetService>();
        //builder.Services.AddTransient<IManualAssetService, ManualAssetService>();
        //builder.Services.AddTransient<ISeedService, SeedService>();
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<ISessionManager, SessionManager>();
        builder.Services.AddTransient<IJobTitleService, JobTitleService>();
        //builder.Services.AddTransient<ISchoolService, SchoolService>();
        //builder.Services.AddTransient<IJpnService, JpnService>();
        //builder.Services.AddTransient<IPpdService, PpdService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddScoped<RoleService>();
        builder.Services.AddScoped<TenantService>();
        builder.Services.AddScoped<AgencyService>();
        builder.Services.AddOptions();
        builder.Services.Configure<AppSettings>(builder.Configuration);
        builder.Services.AddScoped<AgencyGroupingService>();

        builder.Services.AddHttpClient<AgencyService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();

            dbContext.Database.Migrate();


            //await seedService.SeedJpnAsync();
            //await seedService.SeedBahagianAsync();
            //await seedService.SeedPpdAsync();
            //await seedService.SeedAssetMetadata();
            //await seedService.SeedJpnCodeAsync();

            //seedService.Execute();
            //await seedService.SeedSuperadminAsync();

            //await seedService.SeedPTJSchoolAsync();
        }

        app.UseCors(_appPolicy);
        app.UseHsts();
        if (app.Environment.EnvironmentName.ToUpper() == "LOCAL" || app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseJwtTokenMiddleware();
        app.UseAuthorization();

        app.MapControllers();
        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllers();
        //});

        app.Run();
    }
}
