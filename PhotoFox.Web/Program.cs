using PhotoFox.Mappings;
using PhotoFox.Services;
using PhotoFox.Storage;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using IdentityUser = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityUser;
using PhotoFox.Web.Data;

namespace PhotoFox.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddAzureTableStores<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
                {
                    IdentityConfiguration idconfig = new IdentityConfiguration();
                    idconfig.TablePrefix = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:TablePrefix").Value;
                    idconfig.StorageConnectionString = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:StorageConnectionString").Value;
                    idconfig.IndexTableName = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:IndexTableName").Value; // default: AspNetIndex
                    idconfig.RoleTableName = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:RoleTableName").Value;   // default: AspNetRoles
                    idconfig.UserTableName = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:UserTableName").Value;   // default: AspNetUsers
                    return idconfig;
                })).CreateAzureTablesIfNotExists<ApplicationDbContext>();
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

            app.UseAuthentication();
            app.UseAuthorization();
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