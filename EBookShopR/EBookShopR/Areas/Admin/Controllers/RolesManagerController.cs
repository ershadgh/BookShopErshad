using EBookShopR.Areas.Identity.Data;
using EBookShopR.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace EBookShopR.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesManagerController : Controller
    {
        private readonly IApplictionRoleManager _roleManager;
        public RolesManagerController(IApplictionRoleManager roleManager)
        {
            _roleManager= roleManager;
        }
        public IActionResult Index(int PageIndex=1,int row=10)
        {
            //var Roles = _roleManager.Roles.Select(r => new RolesViewModel { RoleID = r.Id, RoleName = r.Name ,Description=r.Description}).ToList();
            var Roles = _roleManager.GetAllRolesAndUsersCount();
            var PaginModel = PagingList.Create(Roles, row, PageIndex);
            PaginModel.RouteValue = new RouteValueDictionary
           {
                {"row",row }
           };
            return View(PaginModel);
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(RolesViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                //    if (await _roleManager.RoleExistsAsync(ViewModel.RoleName))
                //    {
                //        ViewBag.Error = "خطا ! ! !  این نقش تکراری است";
                //    }
                //else
                //{
                //    var Resualt = await _roleManager.CreateAsync(new ApplicationRole(ViewModel.RoleName, ViewModel.Description));
                //    if (Resualt.Succeeded)
                //    {
                //        return RedirectToAction("Index");
                //    }
                //    ViewBag.Error = "در ذخیره اطلاعات خطایی رخ داده است. ";
                //}
                var Result = await _roleManager.CreateAsync(new ApplicationRole(ViewModel.RoleName, ViewModel.Description));
                if (Result.Succeeded)
                {

                    return RedirectToAction("Index");
                    
                }
                foreach (var item in Result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
               
            }

            return View(ViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var Role =await _roleManager.FindByIdAsync(id);
            if (Role==null)
            {
                return NotFound();
            }
            RolesViewModel RoleVM = new RolesViewModel()
            {
               RoleID = Role.Id,
               RoleName=Role.Name,
               Description=Role.Description,
               
            };

            return View(RoleVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(RolesViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {

                var Role =await _roleManager.FindByIdAsync(ViewModel.RoleID);
                if (Role==null)
                {
                    return NotFound();
                }

                Role.Name = ViewModel.RoleName;
                Role.Description = ViewModel.Description;
                var Result = await _roleManager.UpdateAsync(Role);
                if (Result.Succeeded)
                {
                    ViewBag.Success = "ذخیره تغییرات با موفقیت انجام شد.";

                }
                foreach (var item in Result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

            }
            return View(ViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role==null)
            {
                return NotFound();
            }
            RolesViewModel viewModel = new RolesViewModel()
            {
                RoleID = Role.Id,
                RoleName = Role.Name,
            };
            return View(viewModel);
        }
        [ActionName("DeleteRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletedRole(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var Role=await _roleManager.FindByIdAsync(id);
            if (Role==null)
            {
                return NotFound();
            }
            var Result=await _roleManager.DeleteAsync(Role);
            if (Result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Error = "در حذف اطلاعات خطایی رخ داده است";
            RolesViewModel viewModel = new RolesViewModel()
            {
                RoleID = Role.Id,
                RoleName = Role.Name
            };
            return View(viewModel);
        }
    }
}
