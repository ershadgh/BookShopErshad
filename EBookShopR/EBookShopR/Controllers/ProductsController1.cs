using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBookShopR.Controllers
{
    public class ProductsController1 : Controller
    {
        [Authorize("Atleast18")]
       public IActionResult BannedBooks()
        {
            return View();  
        }
    }
}
