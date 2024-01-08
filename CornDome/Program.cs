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
            var config = new Config()
            {
                AppData = new AppData() { DataPath = builder.Configuration["Cards:Data"], ImagePath = builder.Configuration["Cards:Images"] },
                Branding = new Branding() { Title = builder.Configuration["Branding:Title"] },
                ContentStore = new ContentStore() { Articles = builder.Configuration["ContentStore:Articles"], Images = builder.Configuration["ContentStore:Images"] }
            };
            builder.Services.AddSingleton(x => config);
            builder.Services.AddSingleton<ICardRepository, CardRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(config.ContentStore.Images),
                RequestPath = "/EmbeddedImages"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(config.AppData.ImagePath),
                RequestPath = "/CardImages"
            });

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
