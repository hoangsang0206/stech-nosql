using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using System.Security.Claims;

namespace STech.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public AccountController(IUserService userService, IAuthenticationSchemeProvider schemeProvider)
        {
            _userService = userService;
            _schemeProvider = schemeProvider;
        }

        private async Task UserSignIn(User user)
        {
            IEnumerable<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("Avatar", user.Avatar ?? "/images/user-no-image.svg"),
                    new Claim("Id", user.UserId)
                };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue("Id");

            if (userId == null)
            {
                return BadRequest();
            }

            User? user = await _userService.GetUserById(userId);
            if(user == null)
            {
                return BadRequest();
            }

            IEnumerable<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Tài khoản", "")
            };

            return View(new Tuple<User, IEnumerable<Breadcrumb>>(user, breadcrumbs));
        }

        public async Task<IActionResult> ExternalLogin(string? provider, string? returnUrl)
        {
            if (provider == null)
            {
                return BadRequest();
            }

            IEnumerable<AuthenticationScheme> schemes = await _schemeProvider.GetAllSchemesAsync();
            AuthenticationScheme? providerScheme = schemes.FirstOrDefault(s => s.Name == provider);
            if (providerScheme == null)
            {
                return BadRequest();
            }

            AuthenticationProperties properties = new AuthenticationProperties { 
                RedirectUri = Url.Action("ExternalLoginCallback", "Account", new { returnUrl }) 
            };

            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
        {
            returnUrl ??= "/";

            if(remoteError != null)
            {
                return Redirect(returnUrl);
            }

            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                IEnumerable<Claim> claims = result.Principal.Claims;
                string id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
                string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;


                User? user = await _userService.GetUserById(id);
                if (user != null)
                {
                    await HttpContext.SignOutAsync();
                    await UserSignIn(user);
                    return Redirect(returnUrl);
                }

                if (email != null)
                {
                    user = await _userService.GetUserByEmail(email);

                    if (user != null)
                    {
                        await HttpContext.SignOutAsync();
                        await UserSignIn(user);
                        return Redirect(returnUrl);
                    }
                }

                string? name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                string? avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value;
                string? provider = result.Principal.Identity?.AuthenticationType;

                await _userService.CreateUser(new ExternalRegisterVM
                {
                    UserId = id,
                    Email = email,
                    EmailConfirmed = email != null,
                    FullName = name,
                    Avatar = avatar,
                    AuthenticationProvider = provider
                });

                user = await _userService.GetUserById(id);

                await HttpContext.SignOutAsync();
                if (user != null)
                {
                    await UserSignIn(user);
                }

                return Redirect(returnUrl);
            }

            return BadRequest();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
