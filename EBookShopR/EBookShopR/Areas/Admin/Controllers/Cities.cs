using BookShop.Models;
using BookShop.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class Cities : Controller
    {
        private readonly IUnitOfWork _UW;
        public Cities(IUnitOfWork UW)
        {
            _UW = UW;
        }
        public async Task<IActionResult> Index(int id,int PageIndex=1,int row=10)
        {
            if (id==0)
            {
                return NotFound();
            }
            else
            {
                //  _context.Entry(await province).Collection(c => c.City).Query().Where(p=>p.CityName.Contains("تبریز")).Load();            
                //var Book = _context.Books.Where(b => b.BookID == 6).First();
                //_context.Entry(Book).Reference(l => l.Language).Load();
                var province = _UW._Context.Provices.SingleAsync(p=>p.ProvinceID==id);
                 _UW._Context.Entry(await province).Collection(c => c.City).Load();
               // var PagingModel = PagingList.CreateAsync( province, row, PageIndex);

                return View(province.Result);
            }
            
        }
        public IActionResult Create(int? id)
        {
            City cities = new City() { ProvinceID = id };
            return View(cities);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CityID,CityName,ProvinceID")] City city)
        {
            if (ModelState.IsValid)
            {
                Random rdmID=new Random();
                int RandomID = rdmID.Next(500,1000);
                var ExitID =await _UW.BaseRepository<City>().FindByIDAsync(RandomID);
                while(ExitID!=null)
                {
                     RandomID = rdmID.Next(500, 1000);
                     ExitID =await _UW.BaseRepository<City>().FindByIDAsync(RandomID);
                }
               City City=new City() { CityID=RandomID,CityName=city.CityName,ProvinceID=city.ProvinceID};
                _UW.BaseRepository<City>().Create(City);
                _UW.Cmmit();
                return RedirectToAction("Index", new { id = city.ProvinceID });
            }
            return View(city);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var city =await _UW.BaseRepository<City>().FindByIDAsync(id);
            if (city==null)
            {
                return NotFound();
            }
            return View(city);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("CityID,CityName,ProvinceID")] City city)
        {
            if (id!=city.CityID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _UW.BaseRepository<City>().Update(city);
                  await  _UW.Cmmit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _UW.BaseRepository<City>().FindByIDAsync(city.CityID)==null)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index), new { id = city.ProvinceID });
            }
            return View(city);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var city =await _UW.BaseRepository<City>().FindByIDAsync(id);
            if (city==null)
            {
                return NotFound();
            }
            return View(city);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var city =await _UW.BaseRepository<City>().FindByIDAsync(id);
            if (city==null)
            {
                return NotFound();
            }
            else
            {
                _UW.BaseRepository<City>().Delete(city);
                await _UW.Cmmit();
                return RedirectToAction("Index",new {id=city.ProvinceID});
            }
        }
    }
}
