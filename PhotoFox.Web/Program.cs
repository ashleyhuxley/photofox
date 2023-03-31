using PhotoFox.Mappings;
using PhotoFox.Services;
using PhotoFox.Storage;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using IdentityUser = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityUser;
using PhotoFox.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace PhotoFox.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddAzureTableStores<ApplicationDbContext>(new Func<IdentityConfiguration>(() =>
                {
                    IdentityConfiguration idconfig = new()
                    {
                        TablePrefix = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:TablePrefix").Value,
                        StorageConnectionString = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:StorageConnectionString").Value,
                        IndexTableName = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:IndexTableName").Value, // default: AspNetIndex
                        RoleTableName = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:RoleTableName").Value,   // default: AspNetRoles
                        UserTableName = builder.Configuration.GetSection("IdentityAzureTable:IdentityConfiguration:UserTableName").Value   // default: AspNetUsers
                    };
                    return idconfig;
                }))
                .CreateAzureTablesIfNotExists<ApplicationDbContext>()
                .AddSignInManager<UsernameSignInManager>();

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
            builder.Services.AddSingleton<IAlbumPermissionStorage, AlbumPermissionStorage>();
            builder.Services.AddSingleton<IAuthService, AuthService>();

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
            });

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}