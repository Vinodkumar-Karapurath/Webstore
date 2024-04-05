using Microsoft.AspNetCore.Components;
using Webstore.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Webstore.Models;

namespace Webstore.Components
{
    public partial class Login
    {
        [Inject]
        protected UserService userService { get; set; } = default!;

        [SupplyParameterFromForm]
        private LoginViewModels logins { get; set; }

        [CascadingParameter] 
        public HttpContext HttpContext { get; set; }=default!;
        protected override void OnInitialized() { logins = new(); }


        string LastSubmitResult;

        public Login()
        {
        }

        private async Task HandleValidSubmitAsync()
        {
            var user = logins.Username;

            // Process the valid form
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user));
            var principal = new ClaimsPrincipal(identity);
            userService.SetUser(principal);
        }
    }
}