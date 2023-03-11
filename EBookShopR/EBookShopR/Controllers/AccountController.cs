using BookShop.Areas.Identity.Data;
using BookShop.Classes;
using EBookShopR.Areas.Identity.Data;
using EBookShopR.Areas.Identity.Services;
using EBookShopR.Classes;
using EBookShopR.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace EBookShopR.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApplictionRoleManager _roleManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISmsSender _smsSender;
        private readonly ConvertDate _convertDate;
        public AccountController(IApplictionRoleManager roleManager, IApplicationUserManager userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, ISmsSender smsSender, ConvertDate convertDate)
        {
            _convertDate = convertDate;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _smsSender = smsSender;
        }
        [HttpGet]
        public ActionResult Register()
        {

            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                DateTime BirthDateMiladi = _convertDate.ConvertShamsiToMiladi(viewModel.BirthDate);
                var user = new ApplicationUser { UserName = viewModel.UserName, Email = viewModel.Email, PhoneNumber = viewModel.PhoneNumber, RegisterDate = DateTime.Now, IsActive = true, BirthDate = BirthDateMiladi };
                IdentityResult result = await _userManager.CreateAsync(user, viewModel.Password);
                if (result.Succeeded)
                {
                    var Role = await _roleManager.FindByNameAsync("کاربر1");
                    if (Role == null)
                        await _roleManager.CreateAsync(new ApplicationRole("کاربر1", "این نقش متعلق به کابر1 است"));

                    result = await _userManager.AddToRoleAsync(user, "کاربر1");

                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.DateOfBirth, BirthDateMiladi.ToShortDateString()));
                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", values: new { userID = user.Id, code = code }, protocol: Request.Scheme);
                        if (callbackUrl != null)
                            await _emailSender.SendEmailAsync(viewModel.Email, "تایید ایمیل حساب کاربری", $"<div dir:'rtl' style='font-family:tahoma;font-size:'14px'>لطفا با کلیک روی  لینک روبرو ایمیل خود  را تایید کنید.<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک کنید</a> </div>");
                        return RedirectToAction("Index", "Home", new { id = "ConfirmEmail" });
                    }


                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                }
            }
            return View();
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"unable to load user with ID '{userId}'");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Error Confirming email for user with ID'{userId}'");

            return View();


        }
        [HttpGet]
        public IActionResult SignIn(string ReturnUrl = null)
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SignIn(SiginViewModel ViewModel)
        {
            if (Captcha.ValidateCaptchaCode(ViewModel.CaptchaCode, HttpContext))
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(ViewModel.UserName);
                    if (user != null)
                    {
                        if (user.IsActive == true)
                        {
                            var result = await _signInManager.PasswordSignInAsync(ViewModel.UserName, ViewModel.Password, ViewModel.RememberMe, true);
                            if (result.Succeeded)
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            if (result.IsLockedOut)
                            {
                                ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 15 دقیقه به دلیل تلاش های ناموفق قفل شده است");
                                return View();
                            }
                            if (result.RequiresTwoFactor)
                                return RedirectToAction("sendCode", new { Remembreme = ViewModel.RememberMe });
                        }
                    }
                    ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "کد امنیتی صحیح نمی باشد.");
            }

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SignOut()
        {
            var user = await _userManager.GetUserAsync(User);
            user.LastVisitDateTime = DateTime.Now;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {

            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);

            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);

            return new FileStreamResult(s, "imag/png");
        }
        [HttpGet]
        public IActionResult ForGetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForGetPassword(ForgetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(viewModel.Email);
                if (User == null)
                    ModelState.AddModelError(string.Empty, "ایمیل شما صحیح نمی باشد.");
                else
                {
                    if (!await _userManager.IsEmailConfirmedAsync(User))
                        ModelState.AddModelError(string.Empty, "لطفا با تایید ایمیل حساب کاربری خود را فعال کنید");
                    else
                    {
                        var Code = await _userManager.GeneratePasswordResetTokenAsync(User);
                        var CallbackUrl = Url.Action("ResetPassword", "Account", values: new { Code = Code }, protocol: Request.Scheme);
                        await _emailSender.SendEmailAsync(viewModel.Email, "بازیابی کلمه عبور خود", $"<p style='font-family:tahoma;font-size:14px'>برای بازیابی کلمه عبور<a href='{HtmlEncoder.Default.Encode(CallbackUrl)}'>اینحا کلیک کنید</a></p>");
                        return RedirectToAction("ForgetPasswordConfirmation");
                    }

                }
            }
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string Code = null)
        {
            if (Code == null)
                return NotFound();
            else
            {
                var ViewModel = new ResetPasswordViewModel { Code = Code };
                return View(ViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(viewModel.Email);
                if (User == null)
                    ModelState.AddModelError(string.Empty, "ایمیل شما صحیح نمی باشد.");
                else
                {
                    var Result = await _userManager.ResetPasswordAsync(User, viewModel.Code, viewModel.Password);
                    if (Result.Succeeded)
                        return RedirectToAction("ResetPassWordConfirmation");
                    else
                    {
                        foreach (var item in Result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, item.Description);
                        }
                    }
                }
            }
            return View(viewModel);
        }
        public IActionResult ResetPassWordConfirmation()
        {
            return View();
        }
        public async Task<IActionResult> SendSms()
        {
            string Status = await _smsSender.SendAuthSmsAsync("1234", "09361331533");
            if (Status == "success")
                ViewBag.Alert = "ارسال پیامک با موفقیت انجام شد.";
            else

                ViewBag.Alert = "در ارسال پیامک خطایی رخ داده است";

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SendCode(bool RememberMe)
        {
            var FactorOptions = new List<SelectListItem>();
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                return NotFound();

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            foreach (var item in userFactors)
            {
                if (item == "Authenticator")
                {
                    FactorOptions.Add(new SelectListItem { Text = "اپلیکشن احراز هویت", Value = item });
                }
                else
                {
                    FactorOptions.Add(new SelectListItem { Text = (item == "Email" ? "ارسال ایمیل" : "ارسال پیامک"), Value = item });
                }
            }
            //  var FactorOptions = userFactors.Select(p => new SelectListItem { Text = (p == "Email" ? "ارسال ایمیل" : "ارسال پیامک"), Value = p }).ToList();
            return View(new SendCodeViewModel { Providers = FactorOptions, RememberMe = RememberMe });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return NotFound();
            var Code = await _userManager.GenerateTwoFactorTokenAsync(user, viewModel.SelectProvider);
            if (string.IsNullOrWhiteSpace(Code))
                return View("Error");
            var Message = "<p 'style=direction:rtl;font-size:14px;font-family:tahoma'>کد اعتبار سنجی شما " + Code + "</p>";
            if (viewModel.SelectProvider == "Email")
                await _emailSender.SendEmailAsync(user.Email, "کد اعتبار سنجی", Message);
            else if (viewModel.SelectProvider == "Phone")
            {
                string ResponseSmS = await _smsSender.SendAuthSmsAsync(Code, user.PhoneNumber);
                if (ResponseSmS == "Failed")
                {
                    ModelState.AddModelError(string.Empty, "در ارسال پیامک خطایی رخ داده است.");
                    return View(viewModel);
                }


            }
            return RedirectToAction("VerifyCode", new { Provider = viewModel.SelectProvider, Remembreme = viewModel.RememberMe });
        }
        [HttpGet]
        public async Task<IActionResult> VerifyCode(string Provider, bool Rememebreme)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return NotFound();
            return View(new VerifyCodeViewModel { Provider = Provider, RememberMe = Rememebreme });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var Result = await _signInManager.TwoFactorSignInAsync(viewModel.Provider, viewModel.Code, viewModel.RememberMe, viewModel.RememberBrowser);
            if (Result.Succeeded)
                return RedirectToAction("Index", "Home");
            else if (Result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 15 دقیقه قفل شد");

            }
            else
            {
                ModelState.AddModelError(string.Empty, "کد اعتبار سنجی صحیح نمی باشد");
            }
            return View(viewModel);

        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();
            UserSidebarViewModel Sidebar = new UserSidebarViewModel()
            {
                FullName = user.FristName + " " + user.LastName,
                LastVisit = user.LastVisitDateTime,
                RegisterDate = user.RegisterDate,
                Image = user.Image

            };
            return View(new ChangePasswordViewModel { UserSidebar = Sidebar });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var ChangePassResult = await _userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.NewPassword);
            if (ChangePassResult.Succeeded)
                ViewBag.Alert = "کلمه عبور شما با موفقیت تغغیر یافت";
            else
            {
                foreach (var Error in ChangePassResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, Error.Description);
                }
            }
            UserSidebarViewModel Sidebar = new UserSidebarViewModel()
            {
                FullName = user.FristName + " " + user.LastName,
                LastVisit = user.LastVisitDateTime,
                RegisterDate = user.RegisterDate,
                Image = user.Image

            };
            viewModel.UserSidebar = Sidebar;
            return View(viewModel);

        }
        public IActionResult AccessDenied(string ReturnUrl = null)
        {
            return View();

        }
        [Authorize(policy: "HappyBrithDay")]
        public IActionResult HappyBrithDay()
        {
            return View();
        }
    }


}