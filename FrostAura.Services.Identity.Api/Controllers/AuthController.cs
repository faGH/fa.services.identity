using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FrostAura.Services.Identity.Api.Controllers
{
    /// <summary>
    /// Authentication endpoints.
    /// </summary>
    public class AuthController : Controller
    {
        /// <summary>
        /// Identity sign in manager.
        /// </summary>
        private readonly SignInManager<IdentityUser> _signInManager;
        /// <summary>
        /// Identity user manager.
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;
        /// <summary>
        /// Config DB.
        /// </summary>
        private readonly Data.ConfigurationDbContext _configDb;

        /// <summary>
        /// Constructor to allow for injecting variables.
        /// </summary>
        /// <param name="signInManager">Identity sign in manager.</param>
        /// <param name="userManager">Identity user manager.</param>
        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, Data.ConfigurationDbContext configDb)
        {
            _signInManager = signInManager.ThrowIfNull(nameof(signInManager));
            _userManager = userManager.ThrowIfNull(nameof(userManager));
            _configDb = configDb.ThrowIfNull(nameof(configDb));
        }

        /// <summary>
        /// View to prompt for login.
        /// </summary>
        /// <param name="returnUrl">OpenId Connect return url from the query string.</param>
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var request = new LoginViewModel { ReturnUrl = returnUrl };

            request.SetClientClaimsFromConfigDb(_configDb);

            return View(request);
        }

        /// <summary>
        /// Perform login and if successful, redirect to redirect_url.
        /// </summary>
        /// <param name="request">Login request.</param>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel request)
        {
            request.SetClientClaimsFromConfigDb(_configDb);

            if (!ModelState.IsValid) return View(request);

            var response = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

            if (response.Succeeded)
            {
                return Redirect(request.ReturnUrl);
            }

            return View(request);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            var request = new RegistrationViewModel { ReturnUrl = returnUrl };

            request.SetClientClaimsFromConfigDb(_configDb);

            return View(request);
        }

        /// <summary>
        /// Perform registration and if successful, redirect to redirect_url.
        /// </summary>
        /// <param name="request">Registration request.</param>
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationViewModel request)
        {
            request.SetClientClaimsFromConfigDb(_configDb);

            if (!ModelState.IsValid) return View(request);

            var user = new IdentityUser(request.Email);
            var registrationResponse = await _userManager.CreateAsync(user, request.Password);

            if (registrationResponse.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                return Redirect(request.ReturnUrl);
            }

            return View(request);
        }
    }
}
