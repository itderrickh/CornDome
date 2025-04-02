using CornDome.Models;
using CornDome.Repository;
using CornDome.Stores;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            builder.Services.AddScoped<IUserStore<User>, UserStore>();
            builder.Services.AddScoped<IUserRoleStore<User>, UserRoleStore>();

            // Repositories
            builder.Services.AddTransient<IRoleRepository, RoleRepository>();
            builder.Services.AddTransient<ITournamentRepository, TournamentRepository>();
            builder.Services.AddTransient<ICardRepository, SqliteCardRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
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
                //.AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddUserStore<UserRoleStore>()
                .AddSignInManager<SignInManager<User>>();

            builder.Services.AddAuthorization(options =>
            {
                // Define a policy that allows access only to users with the "Admin" role
                options.AddPolicy("admin", policy =>
                    policy.RequireRole("Admin"));  // "Admin" role is required
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Error");
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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();

            app.Run();
        }
    }
}
