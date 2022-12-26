using PhotoFox.Mappings;
using PhotoFox.Services;
using PhotoFox.Storage;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;

namespace PhotoFox.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddSingleton<IPhotoAlbumDataStorage, PhotoAlbumDataStorage>();
            builder.Services.AddSingleton<IPhotoAlbumService, PhotoAlbumService>();
            builder.Services.AddSingleton<IStorageConfig, Config>();
            builder.Services.AddSingleton<IPhotoInAlbumStorage, PhotoInAlbumStorage>();
            builder.Services.AddSingleton<IPhotoMetadataStorage, PhotoMetadataStorage>();
            builder.Services.AddSingleton<IPhotoFileStorage, PhotoFileStorage>();
            builder.Services.AddSingleton(i => MapFactory.GetMap());
            builder.Services.AddSingleton<IPhotoService, PhotoService>();
            builder.Services.AddSingleton<IPhotoHashStorage, PhotoHashStorage>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapBlazorHub();
                //endpoints.MapFallbackToPage("/_Host");
            });

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}