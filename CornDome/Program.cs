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
            builder.Services.AddSingleton<JsonRepositoryConfig>();
            //builder.Services.AddSingleton<SqliteRepositoryConfig>();
            builder.Services.AddSingleton<ICardRepository, JsonCardRepository>();
            //builder.Services.AddSingleton<ICardRepository, SqliteCardRepository>();

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

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
