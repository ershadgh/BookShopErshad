using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Models;
using BookShop.Models.UnitOfWork;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TranslatorsController : Controller
    {
        private readonly IUnitOfWork _UW;

        public TranslatorsController(IUnitOfWork UW)
        {
            _UW=UW;
        }
         
        public async Task<IActionResult>  Index(int pageIndex=1,int row=4)
        {
            var Translators = _UW.BaseRepository<Translator>().FindAllAsync();
            var PagingModel = PagingList.Create(await Translators, row, pageIndex);
            PagingModel.RouteValue = new RouteValueDictionary
            {
                {"row",row }
            };
            return View(PagingModel);
        }

        //public IActionResult Index()
        //{
        //    return View( _context.Translator.ToList());
        //}
      

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TranslatorsCreateViewModel ViewModel)
        {
            if(ModelState.IsValid)
            {
                Translator translator = new Translator()
                {
                    Name = ViewModel.Name,
                    Family=ViewModel.Family,
                };

                _UW.BaseRepository<Translator>().Create(translator);
                await _UW.Cmmit();
                return RedirectToAction("Index");
            }
            return View(ViewModel);
        }


        public async Task<IActionResult>  Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            else
            {
                var Translator = await _UW.BaseRepository<Translator>().FindByIDAsync(id);
                if(Translator==null)
                {
                    return NotFound();
                }

                else
                {
                    return View(Translator);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,[Bind("TranslatorID,Name ,Family")]Translator translator)
        {
            if (id!=translator.TranslatorID)
            {
                return NotFound();
            }
            if(!ModelState.IsValid)
            {
                try
                {
                    _UW.BaseRepository<Translator>().Update(translator);
                    await _UW.Cmmit();
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _UW.BaseRepository<Translator>().FindByIDAsync(translator.TranslatorID) == null)
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(translator);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            else
            {
                var Tranlator = await _UW.BaseRepository<Translator>().FindByIDAsync(id);
                if(Tranlator==null)
                {
                    return NotFound();
                }

                else
                {
                    return View(Tranlator);
                }
            }
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deleted(int? id)
        {
            var Translator = await _UW.BaseRepository<Translator>().FindByIDAsync(id);
            if (Translator==null)
            {
                return NotFound();
            }
            else
            {
                _UW.BaseRepository<Translator>().Delete(Translator);
                await _UW.Cmmit();

                return RedirectToAction("Index");
            }
          
        }
    }
}