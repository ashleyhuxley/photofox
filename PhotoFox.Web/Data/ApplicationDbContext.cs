using ElCamino.AspNetCore.Identity.AzureTable.Model;
using ElCamino.AspNetCore.Identity.AzureTable;

namespace PhotoFox.Web.Data
{
    public class ApplicationDbContext : IdentityCloudContext
    {
        public ApplicationDbContext(IdentityConfiguration config) : base(config)
        {
        }
    }
}