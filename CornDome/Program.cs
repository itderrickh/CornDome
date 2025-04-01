using CornDome.Handlers;
using CornDome.Middleware;
using CornDome.Models;
using CornDome.Repository;
using CornDome.Stores;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;

namespace CornDome
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<Config>();

            // Configurations
            builder.Services.AddSingleton<SqliteRepositoryConfig>();
            builder.Services.AddSingleton<UserRepositoryConfig>();

            builder.Services.AddScoped<IUserStore<User>, UserStore>();

            // Repositories
            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            builder.Services.AddTransient<IRoleRepository, RoleRepository>();
            builder.Services.AddTransient<ILoggingRepository, LoggingRepository>();
            builder.Services.AddTransient<ITournamentRepository, TournamentRepository>();
            builder.Services.AddTransient<ICardRepository, SqliteCardRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IFeedbackRepository, FeedbackRepository>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";  // Change if you have a custom login page
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

            builder.Services.AddIdentity<User, Role>(options =>
            {
                // Identity options here (e.g., password settings)
            })
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddSignInManager<SignInManager<User>>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy =>
                    policy.Requirements.Add(new PermissionRequirement(1))); // 1 = Admin
                options.AddPolicy("tournamentAdmin", policy =>
                    policy.Requirements.Add(new PermissionRequirement(2))); // 2 = TournamentAdmin
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseStatusCodePagesWithRedirects("/Errors/Error{0}");

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(builder.Configuration["ContentStore:Images"]),
                RequestPath = "/EmbeddedImages"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(builder.Configuration["Cards:Images"]),
                RequestPath = "/CardImages"
            });

            if (!app.Environment.IsDevelopment())
                app.UseMiddleware<RouteLogger>();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();

            app.Run();
        }
    }
}
