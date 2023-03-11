using BookShop.Models;
using BookShop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using BookShop.Models.Repository;

namespace BookShop.Models
{
    public class BooksRepository: IBooksRepository 
    {
        readonly private BookShopContext _context;
        public BooksRepository(BookShopContext context)
        {
            _context = context;   
        }
        public List<TreeViewCategory> GetAllCategories(int[] categoryarayy)
        {
      
            var Categories = (from c in _context.Categories
                              where (c.ParentCategoryID == null)
                              select new TreeViewCategory { id = c.CategoryID, title = c.CategoryName,categoryid=categoryarayy }).ToList();
            foreach (var item in Categories)
            {
                BindSubCategory(item,categoryarayy);
            }
          
            return Categories;
        }
        public void BindSubCategory(TreeViewCategory category,int[] arr)
        {
            
            
            
            var SubCategory=(from c in _context.Categories 
                             where(c.ParentCategoryID==category.id)
                             select new TreeViewCategory { id = c.CategoryID, title = c.CategoryName,categoryid=arr});
            foreach (var item in SubCategory)
            {
                BindSubCategory(item,item.categoryid);
                category.Subs.Add(item);
            }
                
        }
        public List<BooksIndexViewModel> GetAllBook( string title, string ISBN, string Author, string Language, string Publisher, string Translator, string Catagory)
        {
            string AuthorsName = "";
            string TranslatorsName = "";
            string CatagoricsyName = "";
            List<BooksIndexViewModel> viewModels = new List<BooksIndexViewModel>();
            var books = (from u in _context.Author_Books.Include(b => b.Book).
                     ThenInclude(p => p.Publisher)
                     .Include(a => a.Author)
                         join L in _context.Languages on u.Book.LanguageID equals L.LanguageID
                         join s in _context.Book_Translators on u.Book.BookID equals s.BookID into bt
                         from bts in bt.DefaultIfEmpty()
                         join t in _context.Translators on bts.TranslatorID equals t.TranslatorID into tr
                         from trl in tr.DefaultIfEmpty()
                         join r in _context.Book_Categories on u.Book.BookID equals r.BookID into bc
                         from bct in bc.DefaultIfEmpty()
                         join c in _context.Categories on bct.CategoryID equals c.CategoryID into cg
                         from cog in cg.DefaultIfEmpty()
                         where (u.Book.Delete == false && u.Book.Title.Contains(title.TrimStart().TrimEnd())
                         && u.Book.ISBN.Contains(ISBN.TrimStart().TrimEnd())
                         && EF.Functions.Like(L.LanguageName, "%" + Language + "%"))
                         && u.Book.Publisher.PublisherName.Contains(Publisher.TrimStart().TrimEnd()) 
                       
                             select new BooksIndexViewModel
                             {

                                 BookID = u.Book.BookID,
                                 Author = u.Author.FirstName + " " + u.Author.LastName,
                                 ISBN = u.Book.ISBN,
                                 Ispublise = u.Book.IsPublish,
                                 Price = u.Book.Price,
                                 PublisheDate = u.Book.PublishDate,
                                 PublusherName = u.Book.Publisher.PublisherName,
                                 Stock = u.Book.Stock,
                                 Title = u.Book.Title,
                                 Translatorr = bts != null ? trl.Name + " " + trl.Family : "",
                                 category = bct != null ? cog.CategoryName : "",
                                
                                 Language = L.LanguageName,

                             }).Where(a=>a.Author.Contains(Author) && a.Translatorr.Contains(Translator)&& a.category.Contains(Catagory)).IgnoreQueryFilters().AsNoTracking().ToList();
            var book1 = books.GroupBy(b => b.BookID).Select(g => new { BookID = g.Key, BookGroups = g }).ToList();
            
            foreach (var item in book1)
            {
                AuthorsName = "";
                TranslatorsName = "";
                CatagoricsyName = "";
                foreach (var group in item.BookGroups.Select(a=>a.Author).Distinct())
                {
                    if (AuthorsName == "" )
                    
                        AuthorsName = group;
                    else
                        AuthorsName = AuthorsName + " - " + group;
                  
                }
                foreach (var group in item.BookGroups.Select(a => a.Translatorr).Distinct())
                {
                    if ( TranslatorsName == "")

                        TranslatorsName = group;
                    else
                        TranslatorsName =TranslatorsName  + " - " + group;

                }
                foreach (var group in item.BookGroups.Select(a => a.category).Distinct())
                {
                    if (CatagoricsyName=="")

                        CatagoricsyName = group;
                    else
                        CatagoricsyName = CatagoricsyName + " - " + group;

                }
                BooksIndexViewModel VM = new BooksIndexViewModel()
                {

                    Author = AuthorsName,
                    Translatorr = TranslatorsName,
                    category= CatagoricsyName,
                    Language=item.BookGroups.First().Language,
                    BookID = item.BookID,
                    ISBN = item.BookGroups.First().ISBN,
                    Title = item.BookGroups.First().Title,
                    Price = item.BookGroups.First().Price,
                    Ispublise = item.BookGroups.First().Ispublise,
                    PublisheDate = item.BookGroups.First().PublisheDate,
                    PublusherName = item.BookGroups.First().PublusherName,
                    Stock = item.BookGroups.First().Stock,

                };
                viewModels.Add(VM);
                

            }
            return viewModels;

        }
    }
}
