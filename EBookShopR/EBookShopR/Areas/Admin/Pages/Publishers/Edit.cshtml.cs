using BookShop.Models;
using BookShop.Models.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookShop.Areas.Admin.Pages.Publishers
{
    
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _UW;
        public EditModel(IUnitOfWork UW)
        {
            _UW=UW;
        }
        [BindProperty]
        public Publisher publisher { get; set; }
        public async Task <IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var IsExitPublisher = await _UW.BaseRepository<Publisher>().FindByIDAsync(id);
            if (IsExitPublisher!=null)
            {
                publisher = await _UW.BaseRepository<Publisher>().FindByIDAsync(id);
                return Page();
            }
            else
            {
                return NotFound();
            }

        }
       
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            else
            {
                var Publeshers =await _UW.BaseRepository<Publisher>().FindByIDAsync(publisher.PublisherID);
                if (Publeshers!=null)
                {
                    Publeshers.PublisherName=publisher.PublisherName;
                  await  _UW.Cmmit();
                    return RedirectToPage("./Index");
                }
                else
                {
                    return NotFound();
                }
            }

        }
    }
}
