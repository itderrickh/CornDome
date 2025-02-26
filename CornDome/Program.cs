using CornDome.Middleware;
using CornDome.Repository;
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

            // Repositories
            builder.Services.AddTransient<ILoggingRepository, LoggingRepository>();
            builder.Services.AddTransient<ITournamentRepository, TournamentRepository>();
            builder.Services.AddTransient<ICardRepository, SqliteCardRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IFeedbackRepository, FeedbackRepository>();

            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Admin/Login"; // Redirect unauthenticated users here
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithRedirects("/Errors/Error{0}");

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
            app.UseAuthorization();
            app.MapRazorPages();

            app.Run();
        }
    }
}
