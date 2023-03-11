using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using BookShop.Models.UnitOfWork;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LanguagesController : Controller
    {
        private readonly IUnitOfWork _UW;
        public LanguagesController(IUnitOfWork UW)
        {
                _UW=UW;
        }
        // GET: Admin/Languages
        public async Task<IActionResult> Index()
        {
            return View(await _UW.BaseRepository<Language>().FindAllAsync());
        }

        // GET: Admin/Languages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _UW.BaseRepository<Language>().FindByIDAsync(id);
               
            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // GET: Admin/Languages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Languages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LanguageID,LanguageName")] Language language)
        {
            if (!ModelState.IsValid)
            {
                _UW.BaseRepository<Language>().Create(language);
                await _UW.Cmmit();
                return RedirectToAction(nameof(Index));
            }
            return View(language);
        }

        // GET: Admin/Languages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _UW.BaseRepository<Language>().FindByIDAsync(id);
            if (language == null)
            {
                return NotFound();
            }
            return View(language);
        }

        // POST: Admin/Languages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LanguageID,LanguageName")] Language language)
        {
            if (id != language.LanguageID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _UW.BaseRepository<Language>().Update(language);
                    await _UW.Cmmit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LanguageExists(language.LanguageID))
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
            return View(language);
        }

        // GET: Admin/Languages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var language = await _UW.BaseRepository<Language>().FindByIDAsync(id);
                
            if (language == null)
            {
                return NotFound();
            }

            return View(language);
        }

        // POST: Admin/Languages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var language = await _UW.BaseRepository<Language>().FindByIDAsync(id);
            _UW.BaseRepository<Language>().Delete(language);
            await _UW.Cmmit();

           // object[] Parameters = { new SqlParameter("@id", id)};
        //   await _context.Database.ExecuteSqlCommandAsync("delete from dbo.Languages where(LanguageID=@id)",Parameters);
           //_context.Database.ExecuteSqlRawAsync("delete from dbo.Languages where (LanguageID = @id)",Parameters);
          //  _context.Books.FromSqlRaw("delete from dbo.Languages where (LanguageID = @id)", Parameters);


            return RedirectToAction(nameof(Index));
        }

        private bool LanguageExists(int id)
        {
            return _UW._Context.Languages.Any(e => e.LanguageID == id);
        }
    }
}
