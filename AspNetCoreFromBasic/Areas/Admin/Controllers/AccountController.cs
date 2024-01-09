using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using AspNetCore.Models;
using System.Security.Claims;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using AspNetCore.DataAccess.Repository.IRepository;
using Newtonsoft.Json;
using Microsoft.DotNet.MSIdentity.Shared;
using AspNetCore.Utilities.Security;
using System.Transactions;
using System.Security.Cryptography;
using AspNetCoreFromBasic.Areas.Customer.Controllers;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UrlEncoder _urlEncoder;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private readonly IUnitOfWork _repo;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment,
            UrlEncoder urlEncoder,
            IUnitOfWork repo)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            _urlEncoder = urlEncoder;
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login()
        {
            LoginModel loginModel = new()
            {
                Input = new InputModel(),
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(loginModel);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Input.Email, loginModel.Input.Password, loginModel.Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    TempData["success"] = "Logged In Successfully";
                    return RedirectToAction(nameof(HomeController.Index), "Home", new {area="Customer"});
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { RememberMe = loginModel.Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return RedirectToAction("LockOut");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(loginModel);
        }
        public async Task<IActionResult> LoginWith2fa(bool rememberMe)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                TempData["error"]="Unable to load two-factor authentication user.";
                return RedirectToAction(nameof(Login));
            }
            LoginWith2faModel loginWith2FaModel = new LoginWith2faModel()
            {
                RememberMe = rememberMe
            };
            return View(loginWith2FaModel);
        }
        [HttpPost]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faModel loginWith2FaModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }
            var authenticatorCode = loginWith2FaModel.Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, loginWith2FaModel.RememberMe, loginWith2FaModel.Input.RememberMachine);
            if (result.Succeeded)
            {
                TempData["success"] = "Logged In Successfully";
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
            }
            else if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }
        public async Task<IActionResult> Register()
        {
            RegisterModel registerModel = new()
            {
                Input = new RegisterInputModel(),
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(registerModel);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.UserName = registerModel.Input.Email;  
                user.Email = registerModel.Input.Email;
                user.PhoneNumber = registerModel.Input.PhoneNumber;
                user.Name = registerModel.Input.Name;
                user.Gender = registerModel.Input.Gender;
                user.Address = registerModel.Input.Address;
                var result = await _userManager.CreateAsync(user, registerModel.Input.Password);
                if (result.Succeeded)
                {
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction(nameof(RegisterConfirmation), new { email = registerModel.Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        TempData["success"] = "User Signed In Successdully";
                        return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });   
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(registerModel);
        }
        public async Task<IActionResult> RegisterConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var EmailConfirmationUrl = Url.Action(
                                    "ConfirmEmail",
                                    "Account",
                                    new { area = "Admin", userId = userId, code = code },
                                    Request.Scheme
                                    );
            await _emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(EmailConfirmationUrl)}'>clicking here</a>.");
            return View();
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                TempData["error"] = "Email could not be verified";
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["error"] = "User not found";
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
            }
            var result=await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["success"] = "Email Confirmed";
            }
            else
            {
                TempData["error"] = "Email Confirmation failure";
            }
            await _signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
        }
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Admin/Account/Register.cshtml");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["success"] = "User logged out successully";
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordModel forgetPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgetPassword.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    TempData["error"] = "Could not reset the password";
                    return View();
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var EmailConfirmationUrl = Url.Action(
                                    "ResetPassword",
                                    "Account",
                                    new { area = "Admin", code = code, email = forgetPassword.Email},
                                    Request.Scheme
                                    );
                await _emailSender.SendEmailAsync(
                    forgetPassword.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(EmailConfirmationUrl)}'>clicking here</a>.");
                return RedirectToAction(nameof(ForgetPasswordConfirmation));
            }
            return View();
        }
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }
        public IActionResult ResetPassword(string code = null, string email = null)
        {
            if (code == null || email==null)
            {
                TempData["error"] = "Problem occurred while resetting the password";
                return RedirectToAction("Login");
            }
            else
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.Code = code;
                model.Email = email;
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["error"] = "Could not reset password";
                return RedirectToAction(nameof(Login));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ExternalLogin(string provider,string returnUrl=null)
        {
            var redirectUrl = Url.Action("Callback", "Account", new { area = "Admin"});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        public async Task<IActionResult> Callback(string remoteError = null)
        {
            if (remoteError != null)
            {
                TempData["error"] = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["error"] = "Error loading external login information.";
                return RedirectToAction(nameof(Login));
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                TempData["success"] = "Logged In using Google";
                return RedirectToAction(nameof(HomeController.Index), "Home", new {area="Customer"});
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }
            else
            {
                ExternalLoginModel externalLoginModel = new ExternalLoginModel();
                externalLoginModel.ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    externalLoginModel.Input = new ExternalLoginInput
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        Name = info.Principal.FindFirstValue(ClaimTypes.Name)
                    };
                }
                return View(nameof(ExternalLogin), externalLoginModel);
            }
        }
        public async Task<IActionResult> ConfirmationExternalAuthentication(ExternalLoginModel externalLoginModel)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["error"] = "Error loading external login information during confirmation.";
                return RedirectToAction(nameof(Login));
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.UserName= externalLoginModel.Input.Email;
                user.Email = externalLoginModel.Input.Email;
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                user.Name = ti.ToTitleCase(externalLoginModel.Input.Name);
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        ApplicationUser _identityuser = _userManager.Users.Where(s => s.Email == externalLoginModel.Input.Email).FirstOrDefault();
                        _identityuser.EmailConfirmed = true;
                        await _userManager.UpdateAsync(_identityuser);
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(nameof(ExternalLogin), externalLoginModel);
        }

        #region Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            
            ProfileIndexModel profileData = new ProfileIndexModel()
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                ImageURL = user.ImageURL
            };
            return View(profileData);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileIndexModel profileData, IFormFile? file)
        {
            var user = await _userManager.FindByEmailAsync(profileData.Email);
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string FileName = Guid.NewGuid().ToString();
                    var FilePath = Path.Combine(wwwRootPath, "images/users");
                    var FileExtension = Path.GetExtension(file.FileName);
                    var FinalPath = Path.Combine(FilePath, FileName + FileExtension);
                    if (profileData.ImageURL != null)
                    {
                        var FileToBeDeleted = Path.Combine(wwwRootPath, profileData.ImageURL.TrimStart('/'));
                        if (System.IO.File.Exists(FileToBeDeleted))
                        {
                            System.IO.File.Delete(FileToBeDeleted);
                        }
                    }
                    using (var fileStream = new FileStream(FinalPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    user.ImageURL = @"/images/users/" + FileName + FileExtension;
                    profileData.ImageURL = user.ImageURL;
                }
                user.Name = profileData.Name;
                user.Address = profileData.Address;
                user.Gender = profileData.Gender;
                user.PhoneNumber = profileData.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["success"] = "User Updated Successfully";
                    return RedirectToAction(nameof(Profile));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View();
        }
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to set password"; 
                return RedirectToAction(nameof(Profile));
            }
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordModel setPassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to set password";
                return RedirectToAction(nameof(Profile));
            }
            var addPasswordResult = await _userManager.AddPasswordAsync(user, setPassword.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _signInManager.RefreshSignInAsync(user);
            TempData["success"] = "Your Password has been set";
            return RedirectToAction(nameof(Profile));
        }
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to set password";
                return RedirectToAction(nameof(Profile));
            }
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to change password";
                return RedirectToAction(nameof(Profile));
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _signInManager.RefreshSignInAsync(user);
            TempData["success"] = "Your Password has been changed";
            return RedirectToAction(nameof(Profile));
        }
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to access Two Factor Authentication";
                return RedirectToAction(nameof(Profile));
            }
            TwoFactorAuthenticationModel twoFactorAuthenticationModel = new TwoFactorAuthenticationModel()
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };
            return View(twoFactorAuthenticationModel);
        }
        [HttpPost]
        public async Task<IActionResult> TwoFactorAuthentication(TwoFactorAuthenticationModel twoFactorAuthenticationModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to access Two Factor Authentication";
                return RedirectToAction(nameof(Profile));
            }
            await _signInManager.ForgetTwoFactorClientAsync();
            TempData["success"] = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
            return RedirectToAction(nameof(Profile));
        }
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to enable Authenticator";
                return RedirectToAction(nameof(Profile));
            }
            EnableAuthenticatorModel enableAuthenticator=await LoadSharedKeyAndQrCodeUriAsync(user);
            return View(enableAuthenticator);
        }
        [HttpPost]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorModel enableAuthenticator)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to enable Authenticator";
                return RedirectToAction(nameof(Profile));
            }
            if (!ModelState.IsValid)
            {
                EnableAuthenticatorModel enableAuthenticatorReset = await LoadSharedKeyAndQrCodeUriAsync(user);
                return View(enableAuthenticator);
            }

            // Strip spaces and hyphens
            var verificationCode = enableAuthenticator.Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                EnableAuthenticatorModel enableAuthenticatorReset = await LoadSharedKeyAndQrCodeUriAsync(user);
                return View(enableAuthenticator);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            TempData["success"] = "Your authenticator app has been verified.";

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                enableAuthenticator.RecoveryCodes = recoveryCodes.ToArray();
                ShowRecoveryCodesModel showRecoveryCodesModel = new ShowRecoveryCodesModel()
                {
                    RecoveryCodes = enableAuthenticator.RecoveryCodes
                };
                return RedirectToAction(nameof(ShowRecoveryCodes), showRecoveryCodesModel);
            }
            else
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }
        }
        public async Task<IActionResult> ShowRecoveryCodes(string[] recoveryCodes)
        {
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            ShowRecoveryCodesModel showRecoveryCodesModel = new ShowRecoveryCodesModel()
            {
                RecoveryCodes = recoveryCodes
            };

            return View(showRecoveryCodesModel);
        }
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to reset Authenticator";
                return RedirectToAction(nameof(Profile));
            }
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            TempData["success"] = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";
            return RedirectToAction(nameof(EnableAuthenticator));
        }
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GenerateRecoveryCodesPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to generate Receovery Code";
                return RedirectToAction(nameof(Profile));
            }
            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!isTwoFactorEnabled)
            {
                TempData["error"] = "Cannot generate recovery codes for user as they do not have 2FA enabled.";
                return RedirectToAction(nameof(Profile));
            }
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            var RecoveryCodesInArray = recoveryCodes.ToArray();
            TempData["success"] = "You have generated new recovery codes.";
            ShowRecoveryCodesModel showRecoveryCodesModel = new ShowRecoveryCodesModel()
            {
                RecoveryCodes = RecoveryCodesInArray 
            };         
            return RedirectToAction(nameof(ShowRecoveryCodes), new { recoveryCodes = showRecoveryCodesModel.RecoveryCodes });
        }
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to Disable 2FA";
                return RedirectToAction(nameof(Profile));
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                TempData["error"] = "Cannot disable 2FA for user as it's not currently enabled";
                return RedirectToAction(nameof(Profile));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Disable2faPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Error occured! Login again to Disable 2FA";
                return RedirectToAction(nameof(Profile));
            }
            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                TempData["error"] = "Error occured! Login again to Disable 2FA";
                return RedirectToAction(nameof(Profile));
            }
            TempData["success"] = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }
        public async Task<IActionResult> LoginWithRecoveryCode()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                TempData["error"] = "Unable to load two-factor authentication user.";
                return RedirectToAction(nameof(Login));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeModel loginWithRecoveryCode)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                TempData["error"] = "Unable to load two-factor authentication user.";
                return RedirectToAction(nameof(Login));
            }
            var recoveryCode = loginWithRecoveryCode.RecoveryCode.Replace(" ", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            if (result.Succeeded)
            {
                TempData["success"] = "Login Successful";
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            }
            return View();
        }
        public async Task<IActionResult> UpdateEmail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Unable to load user";
                return RedirectToAction(nameof(Profile));
            }
            var email = await _userManager.GetEmailAsync(user);
            UpdateEmailModel updateEmail = new UpdateEmailModel()
            {
                Email = email,
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
                Input = new UpdateEmailInputModel
                {
                    NewEmail = email,
                }
            };
            return View(updateEmail);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEmail(UpdateEmailModel updateEmail)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Unable to load user";
                return RedirectToAction("Profile");
            }
            var email = await _userManager.GetEmailAsync(user);
            if (!ModelState.IsValid)
            {
                UpdateEmailModel updateEmailModel = new UpdateEmailModel()
                {
                    Email = email,
                    IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
                    Input = new UpdateEmailInputModel
                    {
                        NewEmail = email,
                    }
                };
                return View(updateEmailModel);
            }
            if (updateEmail.Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, updateEmail.Input.NewEmail);
                var callbackUrl = Url.Action(
                                    "ConfirmEmailChange",
                                    "Account",
                                    new { area = "Admin", userId = userId, email = updateEmail.Input.NewEmail, code = code },
                                    Request.Scheme
                                    );
                await _emailSender.SendEmailAsync(
                    updateEmail.Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                return RedirectToAction(nameof(UpdateEmailConfirmation));
            }
            TempData["error"] = "Your email is unchanged.";
            return RedirectToAction(nameof(Profile));
        }
        public IActionResult UpdateEmailConfirmation()
        {
            return View();
        }
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                TempData["error"] = "Unable to load user";
                return RedirectToAction(nameof(Profile));
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                TempData["error"] = "Error changing Email";
                return RedirectToAction(nameof(Profile));
            }
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                TempData["error"] = "Error changing Username";
                return RedirectToAction(nameof(Profile));
            }
            await _signInManager.RefreshSignInAsync(user);
            TempData["success"] = "Thank you for confirming your email change.";
            return RedirectToAction(nameof(Profile));
        }
        public IActionResult PaymentGatewayEsewa(int id,int count)
        {
            string userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                TempData["error"] = "Login again to continue";
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
            }
            string productId = id.ToString();
            string countOfProduct= count.ToString();
            string dateTime = DateTime.Now.ToString("s");
            string Pid = string.Concat(userId,'_',productId,'_',countOfProduct,'_',dateTime);
            BuyNowViewModel buyNowViewModel = new BuyNowViewModel()
            {
                ProductId = Pid,
                Count = count
            };
            return View(buyNowViewModel);
        }
        public IActionResult PaymentSuccess(string data)
        {
            string decrypedResponse = SHAConfiguration.DecodeHMACSHA256(data);
            EsewaSuccessResponse esewaSuccessReponse = JsonConvert.DeserializeObject<EsewaSuccessResponse>(decrypedResponse)!;
            EsewaPayment esewaPayment = _repo.EsewaPaymentRepo.GetFirstOrDefault(x => x.TransactionId == esewaSuccessReponse.transaction_uuid);
            esewaPayment.TransactionCode = esewaSuccessReponse.transaction_code;
            esewaPayment.Status = esewaSuccessReponse.status;
            _repo.EsewaPaymentRepo.Update(esewaPayment);
            _repo.Save();
            TempData["success"] = "Payment Successful With Esewa";
            return RedirectToAction(nameof(Profile));
        }
        public IActionResult PaymentFailure()
        {
            TempData["error"] = "Payment Failure";
            return RedirectToAction(nameof(Profile));
        }
        public async Task<IActionResult> PaymentGatewayKhalti(int id,int count)
        {
            var url = "https://a.khalti.com/api/v2/epayment/initiate/";
            string userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                TempData["error"] = "Login again to continue";
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
            }
            string productId = id.ToString();
            string countOfProduct = count.ToString();
            string dateTime = DateTime.Now.ToString("s");
            string Pid = string.Concat(userId, '_', productId, '_', countOfProduct, '_', dateTime);
            Product product= _repo.ProductRepo.GetFirstOrDefault(s => s.Id == Convert.ToInt32(productId));
            var payload = new
            {
                return_url = "https://localhost:7139/Admin/Account/PaymentGatewayKhaltiResponse",
                website_url = "https://localhost:7139/",
                amount = Convert.ToString(product.Price*100* Convert.ToInt32(countOfProduct)),
                purchase_order_id = Pid,
                purchase_order_name = product.Title
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Key live_secret_key_68791341fdd94846a146f0457ff7b455");

            var response = await client.PostAsync(url, content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                SuccessResponseKhalti successResponseKhalti = JsonConvert.DeserializeObject<SuccessResponseKhalti>(responseContent)!;
                if (successResponseKhalti != null)
                {
                    PaymentKhalti khaltiPayment = new PaymentKhalti()
                    {
                        Pidx = successResponseKhalti.pidx,
                        Amount = product.Price,
                        Status = "INITIATED"
                    };
                    _repo.PaymentKhaltiRepo.Add(khaltiPayment);
                    _repo.Save();
                    Response.Redirect(successResponseKhalti.payment_url);
                    return View();
                }
                TempData["error"] = "Error occured with Payment by Khalti";
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });
            }
            TempData["error"] = "Error occured with Payment by Khalti";
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Customer" });

        }
        public IActionResult PaymentGatewayKhaltiResponse(PaymentKhaltiResponse khaltiPaymentResponse)
        {
            PaymentKhalti paymentKhalti = _repo.PaymentKhaltiRepo.GetFirstOrDefault(x => x.Pidx == khaltiPaymentResponse.pidx);
            paymentKhalti.Amount = khaltiPaymentResponse.amount/100;
            paymentKhalti.Status = "COMPLETED";
            paymentKhalti.MobileNo = khaltiPaymentResponse.mobile;
            paymentKhalti.TxnId = khaltiPaymentResponse.txnId;
            paymentKhalti.PurchaseOrderId = khaltiPaymentResponse.purchase_order_id;
            paymentKhalti.PurchaseOrderName = khaltiPaymentResponse.purchase_order_name;
            _repo.PaymentKhaltiRepo.Update(paymentKhalti);
            _repo.Save();
            TempData["success"] = "Payment Successful with Khalti";
            return RedirectToAction(nameof(Profile));
        }
        #endregion

        #region Functions
        private async Task<EnableAuthenticatorModel> LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            var email = await _userManager.GetEmailAsync(user);
            EnableAuthenticatorModel enableAuthenticator = new EnableAuthenticatorModel()
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey)
            };
            return enableAuthenticator;
        }
        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Microsoft.AspNetCore.Identity.UI"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
        [HttpGet]
        public JsonResult GetPaymentDetails(string pid)
        {
            string[] parts = pid.Split('_');
            string uid = parts[0];
            string productId= parts[1];
            int count = Convert.ToInt32(parts[2]);
            string time = parts[2];
            //Lets put product count in pid
            string Amount = (_repo.ProductRepo.GetFirstOrDefault(s => s.Id == Convert.ToInt32(productId)).Price * count).ToString("0.00");
            var successURL = Url.Action(
                                    "PaymentSuccess",
                                    "Account",
                                    new { area = "Admin" },
                                    Request.Scheme
                                    );
            var failureURL = Url.Action(
                                    "PaymentFailure",
                                    "Account",
                                    new { area = "Admin" },
                                    Request.Scheme
                                    );
            string ProductCode = "EPAYTEST";
            var message = "total_amount=" + Amount + ",transaction_uuid=" + pid + ",product_code=" + ProductCode;
            PaymentGatewayParam paymentGatewayParam = new PaymentGatewayParam()
            {
                Path = "https://rc-epay.esewa.com.np/api/epay/main/v2/form",
                Params = new PaymentGatewayParamInner()
                {
                    amount = Amount,
                    tax_amount = "0",
                    total_amount = Amount,
                    transaction_uuid = pid,
                    product_code = ProductCode,
                    product_service_charge = "0",
                    product_delivery_charge = "0",
                    success_url = successURL,
                    failure_url = failureURL,
                    signed_field_names = "total_amount,transaction_uuid,product_code",
                    signature = SHAConfiguration.ComputeHMACSHA256(message, "8gBm/:&EnhH.1/q")
                }
            };
            EsewaPayment esewaPayment = new EsewaPayment()
            {
                UserId = uid,
                TransactionId = pid,
                Status = "Initiated",
                TotalAmount = Convert.ToDecimal(Amount),
                ProductCode = ProductCode,
                ProductId = Convert.ToInt32(productId),
                Count = count,
            };
            _repo.EsewaPaymentRepo.Add(esewaPayment);
            _repo.Save();
            return Json(paymentGatewayParam);
        }
        #endregion
    }
}
