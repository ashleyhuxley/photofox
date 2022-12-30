using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using IdentityUser = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityUser;

namespace PhotoFox.Web
{
    public class UsernameSignInManager : SignInManager<IdentityUser>
    {
        public UsernameSignInManager(
            UserManager<IdentityUser> userManager, 
            IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<IdentityUser>> logger, 
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<IdentityUser> userConfirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, userConfirmation)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password,
            bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
    }
}
