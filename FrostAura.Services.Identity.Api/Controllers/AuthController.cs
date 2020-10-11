using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Services.Identity.Data.Models;
using FrostAura.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System;
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
        private readonly SignInManager<FaUser> _signInManager;
        /// <summary>
        /// Identity user manager.
        /// </summary>
        private readonly UserManager<FaUser> _userManager;
        /// <summary>
        /// Config DB.
        /// </summary>
        private readonly Data.ConfigurationDbContext _configDb;
        /// <summary>
        /// Email service provider.
        /// </summary>
        private readonly IEmailService _emailService;
        /// <summary>
        /// Whether email verification is required.
        /// </summary>
        private readonly bool _enableEmailVerification = false;

        /// <summary>
        /// Constructor to allow for injecting variables.
        /// </summary>
        /// <param name="signInManager">Identity sign in manager.</param>
        /// <param name="userManager">Identity user manager.</param>
        /// <param name="configDb">Configuration db.</param>
        /// <param name="emailService">Email service provider.</param>
        public AuthController(SignInManager<FaUser> signInManager, UserManager<FaUser> userManager, Data.ConfigurationDbContext configDb, IEmailService emailService)
        {
            _signInManager = signInManager.ThrowIfNull(nameof(signInManager));
            _userManager = userManager.ThrowIfNull(nameof(userManager));
            _configDb = configDb.ThrowIfNull(nameof(configDb));
            _emailService = emailService.ThrowIfNull(nameof(emailService));
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

            var user = await _userManager.FindByNameAsync(request.Email);

            if (user != default)
            {
                #region Confirmation & Reset

                if (_enableEmailVerification && !user.EmailConfirmed)
                {
                    // Same for password reset.
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var redirectUrl = Url.Action(nameof(VerifyEmail), "Auth", new { userId = user.Id, token = emailConfirmationToken, returnUrl = request.ReturnUrl }, Request.Scheme);

                    await _emailService.SendAsync("deanmar@outlook.com", "Confirm Account Email", $"<a href='{redirectUrl}'>Confirm Email</a>", isHtml: true);

                    // TODO: Replace with a view.
                    return Ok("Email verification email sent!");
                }

                #endregion

                var response = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

                if (response.Succeeded)
                {
                    return Redirect(request.ReturnUrl);
                }
            }

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId, string token, string returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == default) return BadRequest();

            var confirmationResult = await _userManager.ConfirmEmailAsync(user, token);

            if (confirmationResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                return Redirect(returnUrl);
            }

            throw new NotImplementedException();
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

            var user = new FaUser
            {
                UserName = request.Email,
                Email = request.Email
            };
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
