using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class Sample : Controller
    {
        private readonly BookShopContext _context;
        public Sample(BookShopContext context)
        {
            _context= context;
        }
        public IActionResult Index()
        {
            System.Diagnostics.Stopwatch stopwatch= new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            var query = EF.CompileAsyncQuery( (BookShopContext context, int id) 
                => context.Books.SingleOrDefault(b => b.BookID==id));

            for (int i = 0; i < 1000; i++)
            {
                ///var book = _context.Books.SingleOrDefault(p => p.BookID == i);
               var book = query(_context, i);
            }
            stopwatch.Stop();
            return View(stopwatch.ElapsedMilliseconds);
        }
    }
}
