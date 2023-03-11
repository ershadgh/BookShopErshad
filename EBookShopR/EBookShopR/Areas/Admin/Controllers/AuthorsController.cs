using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BookShop.Models.Repository;
using BookShop.Models.UnitOfWork;
using BookShop.Models.ViewModels;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorsController : Controller
    {
        private readonly IUnitOfWork _UW;
        public AuthorsController(IUnitOfWork UW)
        {
            _UW = UW;
        }

        public async Task<IActionResult> Index()
        {

            var Authors = _UW.BaseRepository<Author>().FindAllAsync();

           // var Authors = _UW.BaseRepository<Author>().FindByConditionAsync(o => o.FirstName.Contains("ع"),b=>b.OrderBy(k=>k.LastName),o=>o.Author_Books);
            return View(await Authors);
        }

        // GET: Admin/Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var author=await _UW.BaseRepository<Author>().FindByIDAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        public IActionResult Create()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (!ModelState.IsValid)
            {
            await  _UW.BaseRepository<Author>().Create(author);

                await _UW.Cmmit();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Admin/Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _UW.BaseRepository<Author>().FindByIDAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorID,FirstName,LastName")] Author author)
        {
            if (id != author.AuthorID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    _UW.BaseRepository<Author>().Update(author);



                    await _UW.Cmmit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_UW.BaseRepository<Author>().FindByIDAsync(author.AuthorID)==null)
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
            return View(author);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _UW.BaseRepository<Author>().FindByIDAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           var author= await _UW.BaseRepository<Author>().FindByIDAsync(id);
            if (author==null)
            {
                return NotFound();
            }
            else
            {
                _UW.BaseRepository<Author>().Delete(author);
                await _UW.Cmmit();
                return RedirectToAction(nameof(Index));
            }
          
        }

        //private bool AuthorExists(int id)
        //{
        //    return _context.Authors.Any(e => e.AuthorID == id);
        //}


        private List<EntityStates> DisplayStates(IEnumerable<EntityEntry> entities)
        {
            List<EntityStates> EntityStates = new List<EntityStates>();
            foreach(var entry in entities)
            {
                EntityStates states = new EntityStates()
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityState=entry.State.ToString(),
                };

                EntityStates.Add(states);
            }

            return EntityStates;
        }
        public async Task<ActionResult> AuthorBooks(int id)
        {

           // var Authors =  (from a in _UW._Context.Author_Books.Include(c=>c.Author).Include(b=>b.Book).ThenInclude(p=>p.Publisher) where
           //                 (a.Author.AuthorID==id) join l in _UW._Context.Languages on a.Book.LanguageID equals l.LanguageID
           //                 select new BooksIndexViewModel { BookID=a.BookID,Title=a.Book.Title,ISBN=a.Book.ISBN,PublusherName=a.Book.Publisher.PublisherName,Language=l.LanguageName,Author=a.Author.FirstName+""+a.Author.LastName}).ToList();
           var Authors =await _UW.BaseRepository<Author>().FindByIDAsync(id);
           //  _UW._Context.Entry(await Authors).Collection(c=>c.Author_Books).Load();
            

            if (Authors==null)
            {
                return NotFound();
            }
            else
            {
                return View( Authors);
            }

           
        }

    }
}
