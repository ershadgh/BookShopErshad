using EBookShopR.Areas.Identity.Data;
using EBookShopR.Classes;
using EBookShopR.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace EBookShopR.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles ="مدیر سایت")]
    public class UserManagerController : Controller
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IApplictionRoleManager _roleManager;
        private readonly IConvertDate _convertDate;
        private readonly IEmailSender _emailSender;
        public UserManagerController(IApplicationUserManager userManager,IApplictionRoleManager roleManager, IConvertDate convertDate,IEmailSender emailSender)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _convertDate=convertDate;
            _emailSender = emailSender;

        }
       // [Authorize(Roles = "مدیر سایت,کاربر1")]
      [Authorize(Policy = "AccessToUserManager")]
        public async Task<IActionResult> Index(string Msg,int PageIndex=1,int row=10)
        {
            if (Msg== "Success")


                ViewBag.Alert = "عضویت با موفقیت انجام شد.";

            if (Msg == "SendEmaliSuccess")
                ViewBag.Alert = "ارسال ایمیل به کاربران با موفقیت انجام شد";
            var PageModel=PagingList.Create(await _userManager.GetAllUserWhitRoleAsync(),row,PageIndex);
            return View(PageModel);
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();
            else
            {
                var user = await _userManager.FindUserWithRoleByIdAsync(id);
                if (user == null)
                    return NotFound();
                else
                    return View(user);
                
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id==null)
                return NotFound();
            var user =await _userManager.FindUserWithRoleByIdAsync(id);
            if (user == null)
                return NotFound();
            else
            {
                ViewBag.AllRoles = _roleManager.GetAllRoles();
                if (user.BirthDate!=null)
                user.PersianBirthDate = _convertDate.ConverMiladitoShamsi((DateTime)user.BirthDate,"yyyy/MM/dd");
                return View(user);
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(UsersViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var User= await _userManager.FindByIdAsync(viewModel.Id);
                if (User == null)
                    return NotFound();
                else
                {
                    IdentityResult Result;
                    var RecentRoles = await _userManager.GetRolesAsync(User);
                    var DeleteRoles=RecentRoles.Except(viewModel.Roles);
                    var AddRoles=viewModel.Roles.Except(RecentRoles);
                    Result=await _userManager.RemoveFromRolesAsync(User, DeleteRoles);
                    if (Result.Succeeded)
                    {
                        Result = await _userManager.AddToRolesAsync(User, AddRoles);
                        if (Result.Succeeded)
                        {
                            User.FristName = viewModel.FirstName;
                            User.LastName = viewModel.LastName;
                            User.Email = viewModel.Email;
                            User.PhoneNumber = viewModel.PhoneNumber;
                            User.UserName = viewModel.UserName;
                            User.BirthDate = _convertDate.ConvertShamsiToMiladi(viewModel.PersianBirthDate);
                            Result=await _userManager.UpdateAsync(User);
                            if (Result.Succeeded)
                            {
                                ViewBag.AlertSucceed = "ذخیره تغییرات با موفقیت انجام شد";
                            }
                        }

                    }
                    if (Result!=null)
                    {
                        foreach (var item in Result.Errors)
                        {
                            ModelState.AddModelError("",item.Description);
                        }
                    }
                }
            }
            ViewBag.AllRoles = _roleManager.GetAllRoles();
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id==null)
            
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            else
                return View(user);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Deleted(string id)
        {
            if (id == null)
                return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            else
            {
                var Result=await _userManager.DeleteAsync(user);
                if (Result.Succeeded)
                    return RedirectToAction("Index");

                else
                    ViewBag.AlertError = "در حذف اطلاعات خطایی رخ داده است";
                return View(user);
            }
            
        }
        public async Task<IActionResult> SendEmail(string[] emails,string subject,string message)
        {
            if (emails!=null)
            {
                for (int i = 0; i < emails.Length; i++)
                {
                    await _emailSender.SendEmailAsync(emails[i], subject, message);
                }
               
            }
            return RedirectToAction("Index", new { Msg = "SendEmaliSuccess" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeLockOutEnable(string UserId,bool Status)
        {
            var User = await _userManager.FindByIdAsync(UserId);
            if (User == null)
            {
                return NotFound();
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(User, Status);
                return RedirectToAction("Details", new { id = UserId });
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUserAccount(string UserId)
        {
            var User = await _userManager.FindByIdAsync(UserId);
            if(User==null)
            {
                return NotFound();
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(User,DateTimeOffset.UtcNow.AddMinutes(20));
                return RedirectToAction("Details", new { id = UserId });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLockUserAccount(string UserId)
        {
            var User = await _userManager.FindByIdAsync(UserId);
            if(User==null)
            {
                return NotFound();
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(User, null);
                return RedirectToAction("Details", new { id = UserId });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> IActiveOrActiveUser(string UserId,bool Status)
        {
            var User = await _userManager.FindByIdAsync(UserId);
            if(User==null)
            {
                return NotFound();
            }
            User.IsActive = Status;
            await _userManager.UpdateAsync(User);
            return RedirectToAction("Details", new { id = UserId });
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string UserId)
        {
            var User = await _userManager.FindByIdAsync(UserId);
            if(User==null)
            {
                return NotFound();
            }
            var ViewModel = new UserResetPasswordViewModel { Id = User.Id, UserName = User.UserName, Email = User.Email };
            return View(ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserResetPasswordViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var User=await _userManager.FindByIdAsync(viewModel.Id);
                if(User==null)
                {
                    return NotFound();
                }
                await _userManager.RemovePasswordAsync(User);
                await _userManager.AddPasswordAsync(User, viewModel.NewPassword);
                ViewBag.AlertSuccess = "بازنشانی کلمه عبور با موفقیت انجام شد";
                viewModel.UserName = User.UserName;
                viewModel.Email = User.Email;
               
            }
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeTowFactorEnable(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if(user==null)
            {
                return NotFound();
            }
            if (user.TwoFactorEnabled)
                user.TwoFactorEnabled = false;
            else
                user.TwoFactorEnabled = true;
            var Result = await _userManager.UpdateAsync(user);
            if (!Result.Succeeded)
            {
                foreach (var item in Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            return RedirectToAction("Details", new { id = UserId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmailConfirm(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if(UserId==null)
            {
                return NotFound();
            }
            if (user.EmailConfirmed)
                user.EmailConfirmed = false;
            else
                user.EmailConfirmed = true;
          var Result=await _userManager.UpdateAsync(user);
            if(!Result.Succeeded)
            {
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Details", new { id = UserId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhonNumberConfirm(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if(user==null)
            {
                return NotFound();
            }
            if (user.PhoneNumberConfirmed)
                user.PhoneNumberConfirmed = false;
            else
                user.PhoneNumberConfirmed = true;
            var Result= await _userManager.UpdateAsync(user);
            if(!Result.Succeeded)
            {
                foreach (var error in Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction("Details", new { id = UserId });
        }
        
    }
}
