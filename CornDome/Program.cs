using CornDome.Helpers;
using CornDome.Models.Users;
using CornDome.Repository;
using CornDome.Repository.Discord;
using CornDome.Repository.Tournaments;
using CornDome.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Net;

namespace CornDome
{
    public class Program
    {
        public const string POOLING = ";Pooling=False";
        public static void AddRepositories(IServiceCollection services)
        {
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<ICardRepository, CardRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            services.AddTransient<IUserRoleRepository, UserRoleRepository>();
            services.AddTransient<IDiscordRepository, DiscordRepository>();
            services.AddTransient<IBugReportRepository, BugReportRepository>();
            services.AddTransient<ILogEntryRepository, LogEntryRepository>();
            services.AddTransient<IDeckRepository, DeckRepository>();
        }

        public static void AddDbContext(WebApplicationBuilder builder, IServiceCollection services)
        {
            services.AddDbContext<MainContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString(AppConstants.MASTER_DB);
                options.UseSqlite(connectionString + POOLING);
            });

            services.AddDbContext<CardDatabaseContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString(AppConstants.CARDS_DB);
                options.UseSqlite(connectionString + POOLING);
            });
            services.AddDbContext<TournamentContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString(AppConstants.TOURNAMENT_DB);
                options.UseSqlite(connectionString + POOLING);
            });
        }

        public static void AddStaticRoutes(WebApplicationBuilder builder, WebApplication app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(builder.Configuration["Cards:Images"]),
                RequestPath = AppConstants.CARD_STATIC_IMAGE_ROUTE
            });
        }

        public static void AddAuthentication(WebApplicationBuilder builder)
        {
            string clientId = !string.IsNullOrEmpty(builder.Configuration["Authentication:Google:ClientId"])
                ? builder.Configuration["Authentication:Google:ClientId"]
                : Environment.GetEnvironmentVariable("Authentication__Google__ClientId");

            string clientSecret = !string.IsNullOrEmpty(builder.Configuration["Authentication:Google:ClientSecret"])
                ? builder.Configuration["Authentication:Google:ClientSecret"]
                : Environment.GetEnvironmentVariable("Authentication__Google__ClientSecret");

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                });
        }

        public static void AddAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserStore<User>, UserStore>();
            builder.Services.AddScoped<IUserRoleStore<User>, UserRoleStore>();

            builder.Services.AddIdentity<User, Role>(options =>
            {
                // Identity options here (e.g., password settings)
            })
                .AddDefaultTokenProviders()
                .AddRoleStore<RoleStore>()
                .AddUserStore<UserRoleStore>()
                .AddSignInManager<SignInManager<User>>();

            builder.Services.AddAuthorization(options =>
            {
                // Define a policy that allows access only to users with the "Admin" role
                options.AddPolicy("admin", policy =>
                    policy.RequireRole(AppConstants.ROLE_ADMIN));
                options.AddPolicy("tournamentOrganizer", policy =>
                    policy.RequireRole(AppConstants.ROLE_TOURNAMENT_MANAGER, AppConstants.ROLE_ADMIN));
                options.AddPolicy("cardManager", policy =>
                    policy.RequireRole(AppConstants.ROLE_CARD_MANAGER, AppConstants.ROLE_ADMIN));
                options.AddPolicy("rulingManager", policy =>
                    policy.RequireRole(AppConstants.ROLE_RULING_MANAGER, AppConstants.ROLE_ADMIN));
            });
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();
            
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<Config>();

            var keyFolder = builder.Environment.IsDevelopment()
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConstants.APP_NAME, "DataProtectionKeys")
                : "/var/myapp/keys";  // Linux production path

            builder.Services.AddDataProtection()
               .PersistKeysToFileSystem(new DirectoryInfo(keyFolder))
               .SetApplicationName(AppConstants.APP_NAME);  // must be consistent
            builder.Services.AddTransient<ITokenProtector, TokenProtector>();
            builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(AppConstants.DISCORD_TOKEN_ROUTE);
                client.Timeout = TimeSpan.FromSeconds(30);
            });


            AddDbContext(builder, builder.Services);
            AddRepositories(builder.Services);
            AddAuthentication(builder);

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;

                // Only if you trust the proxy and need to clear defaults:
                options.KnownProxies.Add(IPAddress.Parse(AppConstants.LOCALHOST));
            });

            builder.Services.AddSingleton<ICardChangeLogger, CardChangeLogger>();

            AddAuthorization(builder);

            var app = builder.Build();

            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Errors/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");

            AddStaticRoutes(builder, app);

            app.UseMiddleware<ErrorLoggerMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();

            app.Run();
        }
    }
}
