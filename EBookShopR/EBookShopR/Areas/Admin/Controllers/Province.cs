using BookShop.Models;
using BookShop.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class Province : Controller
    {
        private readonly IUnitOfWork _UW;
        public Province(IUnitOfWork UW)
        {
            _UW = UW;
        }
        public async Task<IActionResult> Index(int PageIndex=1,int row=10)
        {
            var Provice=_UW.BaseRepository<Provice>().FindAllAsync();
            var PagingModel = PagingList.Create(await Provice, row, PageIndex);
            PagingModel.RouteValue = new RouteValueDictionary()
            {
                {"row",row }
            };
            return View(PagingModel);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvinceID,ProvinceName")] Provice provice)
        {
            if (ModelState.IsValid)
            {
                Random rdm = new Random();
                int RandomID = rdm.Next(32, 1000);
                var ExitID=await _UW.BaseRepository<Provice>().FindByIDAsync(RandomID);
                while (ExitID != null)
                {
                     RandomID = rdm.Next(32, 1000);
                    ExitID =await _UW.BaseRepository<Provice>().FindByIDAsync(RandomID);
                }
                Provice Provice=new Provice() { ProvinceName=provice.ProvinceName,ProvinceID=RandomID};
               await _UW.BaseRepository<Provice>().Create(Provice);
                await _UW.Cmmit();
                return RedirectToAction("Index");
            }
            return View(provice);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
             var provinces =await _UW.BaseRepository<Provice>().FindByIDAsync(id);
            if (provinces==null)
            {
                return NotFound();
            }
            return View(provinces);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,[Bind("ProvinceID, ProvinceName")]Provice provice)
        {
            if (provice.ProvinceID!=id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    
                    _UW.BaseRepository<Provice>().Update(provice);
                    await _UW.Cmmit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _UW.BaseRepository<Provice>().FindByIDAsync(provice.ProvinceID) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(provice);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var province =await _UW.BaseRepository<Provice>().FindByIDAsync(id);
            if (province==null)
            {
                return NotFound();
            }
            return View(province);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var province =await _UW.BaseRepository<Provice>().FindByIDAsync(id);
            if (province==null)
            {
                return NotFound();
            }
            else
            {
                _UW.BaseRepository<Provice>().Delete(province);
                await _UW.Cmmit();
                return RedirectToAction("Index");
            }
        }
    }
}
