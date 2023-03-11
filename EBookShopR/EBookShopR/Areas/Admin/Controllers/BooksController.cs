using BookShop.Models;
using BookShop.Models.UnitOfWork;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Diagnostics;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
   
    public class BooksController : Controller
    {
        private readonly IUnitOfWork _UW;
       

        public BooksController(IUnitOfWork UW, BooksRepository repository)
        {
            _UW = UW;
           
        }
        public IActionResult Index(int pageIndex=1,int row=5,string sortExpression= "Title",string title="")
        {
            
            title = string.IsNullOrEmpty(title) ? "" : title;
           
            List<int> Rows = new List<int>()
            {
                2, 5,10,15,20,50,100
            };
            ViewBag.RowID= new SelectList(Rows,row);
            ViewBag.Search = title;
           

            //var books = (from c in _context.Books join p in _context.Publishers
            //             on c.PublisherID equals p.PublisherID join 
            //             AB in _context.Author_Books on c.BookID equals AB.BookID join
            //             A in _context.Authors on AB.AuthorID equals A.AuthorID
            //             where(c.Delete==false) 
            //             select new BooksIndexViewModel 
            //             {
            //                 BookID=c.BookID,
            //                 ISBN=c.ISBN,Ispublise=c.IsPublish,
            //                 PublusherName=p.PublisherName,
            //                 Price=c.Price,PublisheDate=c.PublishDate,
            //                 Stock=c.Stock,Title=c.Title,Author=A.FirstName +" "+ A.LastName }
            //             ).ToList();
            
            var PagingModel = PagingList.Create(_UW.booksRepository.GetAllBook(title,"","","","","",""),row,pageIndex, sortExpression, "Title");
             PagingModel.RouteValue = new RouteValueDictionary
            {
                {"row",row }
            };
            ViewBag.PageofRow=(pageIndex-1)*row+1;
            int[] aray = null;
            ViewBag.Categories = _UW.booksRepository.GetAllCategories(aray);
            ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageName", "LanguageName");
            ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(a => new AuthorList { AuthorID = a.AuthorID, NameFamily = a.FirstName + " " + a.LastName }), "NameFamily", "NameFamily" );
            ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherName", "PublisherName");
            ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + " " + t.Family }), "NameFamily", "NameFamily");

            return View(PagingModel);

        }
        public IActionResult AdvansedSearch(AdvancedSearch advancedSearch)
        {
            advancedSearch.Title=String.IsNullOrEmpty(advancedSearch.Title)?"":advancedSearch.Title;
            advancedSearch.ISBN = String.IsNullOrEmpty(advancedSearch.ISBN) ? "" : advancedSearch.ISBN;
            advancedSearch.Author = String.IsNullOrEmpty(advancedSearch.Author) ? "" : advancedSearch.Author;
            advancedSearch.Language = String.IsNullOrEmpty(advancedSearch.Language) ? "" : advancedSearch.Language;
            advancedSearch.Publisher = String.IsNullOrEmpty(advancedSearch.Publisher) ? "" : advancedSearch.Publisher;
            advancedSearch.Translator = String.IsNullOrEmpty(advancedSearch.Translator) ? "" : advancedSearch.Translator;
            advancedSearch.Catagory = String.IsNullOrEmpty(advancedSearch.Catagory) ? "" : advancedSearch.Catagory;
            var Books = _UW.booksRepository.GetAllBook(advancedSearch.Title,advancedSearch.ISBN,advancedSearch.Author,advancedSearch.Language,advancedSearch.Publisher,advancedSearch.Translator,advancedSearch.Catagory);
            return View(Books);
        }
        public IActionResult Create()
        {
            
            ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
            ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
            ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + "" + t.LastName }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + "" + t.Family }), "TranslatorID", "NameFamily");
            BooksCreateEditViewModel viewModel = new BooksCreateEditViewModel(_UW.booksRepository.GetAllCategories(null));
            
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksCreateEditViewModel ViewModel)
        {


            //List<Author_Book> authors=new List<Author_Book>();

            int[] ara = null;
            var TransAction = _UW._Context.Database.BeginTransaction();
            
            if (ModelState.IsValid)
            {
                List<Book_Translator> translators = new List<Book_Translator>();
                if (ViewModel.TranslatorID != null)
                {
                    translators = ViewModel.TranslatorID.Select(a => new Book_Translator { TranslatorID = a }).ToList();
                }
                List<Book_Category> categories = new List<Book_Category>();
                if (ViewModel.CategoryID != null)
                {
                    categories = ViewModel.CategoryID.Select(a => new Book_Category { CategoryID = a }).ToList();
                }
                DateTime? publishDate = null;
                if (ViewModel.IsPublish = true)
                {
                    publishDate = DateTime.Now;
                }
                try
                {
                    Book book = new Book()
                    {

                       // Delete = false,
                         
                        ISBN = ViewModel.ISBN,
                        IsPublish = ViewModel.IsPublish,
                        NumOfPages = ViewModel.NumOfPages,
                        Stock = ViewModel.Stock,
                        LanguageID = ViewModel.LanguageID,
                        Summary = ViewModel.Summary,
                        PublishYear = ViewModel.PublishYear,
                        Title = ViewModel.Title,
                        PublishDate = publishDate,
                        Weight = ViewModel.Weight,
                        PublisherID = ViewModel.PublisherID,
                        Price = ViewModel.Price,
                        File = ViewModel.File,
                        book_Tranlators=translators,
                        book_Categories = categories,
                        Author_Books = ViewModel.AuthorID.Select(a => new Author_Book { AuthorID = a }).ToList(),
                    };

                    await _UW.BaseRepository<Book>().Create(book);
                    //  await _context.SaveChangesAsync();


                    // if (ViewModel.TranslatorID != null)
                    //{
                    //     for (int i = 0; i < ViewModel.TranslatorID.Length; i++)
                    //     {
                    //         Book_Translator translator = new Book_Translator()
                    //         {
                    //             BookID = book.BookID,
                    //             TranslatorID = ViewModel.TranslatorID[i]
                    //         };
                    //         translators.Add(translator);
                    //     }
                    //     await _context.Book_Translators.AddRangeAsync(translators);
                    //     await _context.SaveChangesAsync();
                    // }

                    //if (ViewModel.AuthorID != null)
                    //{
                    //    for (int i = 0; i < ViewModel.AuthorID.Length; i++)
                    //    {

                    //        Author_Book author = new Author_Book()
                    //        {
                    //            BookID = book.BookID,
                    //            AuthorID = ViewModel.AuthorID[i],
                    //        };
                    //        authors.Add(author);
                    //    }
                    //    await _context.Author_Books.AddRangeAsync(authors);
                    //    await _context.SaveChangesAsync();
                    //}

                    //if (ViewModel.CategoryID != null)
                    //{
                    //    for (int i = 0; i < ViewModel.CategoryID.Length; i++)
                    //    {
                    //        Book_Category category = new Book_Category()
                    //        {
                    //            BookID = book.BookID,
                    //            CategoryID = ViewModel.CategoryID[i],
                    //        };
                    //        categories.Add(category);
                    //    }
                    //    await _context.Book_Categories.AddRangeAsync(categories);

                    //}
                    await _UW.Cmmit();
                    TransAction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {

                    return RedirectToAction("Index", new { Msg = "failed" });
                }

            }

            else
            {

                ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
                ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
                ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + "" + t.LastName }), "AuthorID", "NameFamily");
                ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + "" + t.Family }), "TranslatorID", "NameFamily");
                ViewModel.Categories = _UW.booksRepository.GetAllCategories(ara);
                
                return View(ViewModel);
            }



        }
        public IActionResult Details(int id)
        {

            //var BookInfo = _context.Books.FromSqlRaw("Select *From dbo.BookInfo Where BookId={0}", id)
            //    .Include(l => l.Language).Include(p => p.Publisher).First();
            //ViewBag.Authors = _context.Authors.FromSqlRaw("EXEC dbo.GetAllAuthrBookIDBy {0}", id).ToList();
            //ViewBag.Translators = (from bt in _context.Book_Translators
            //                      join t in _context.Translators on bt.TranslatorID equals t.TranslatorID
            //                      where (bt.BookID == id)
            //                      select new Translator
            //                      {
            //                          Name = t.Name,
            //                          Family = t.Family
            //                   }).ToList();
            //ViewBag.Categories = (from b in _context.Book_Categories
            //                      join c in _context.Categories on b.CategoryID equals c.CategoryID
            //                      where (b.BookID == id)
            //                      select new Category { CategoryName = c.CategoryName }).ToList();
            var BookInfo = _UW._Context.ReadAllBooks.FromSqlRaw("SELECT  b.BookID, b.ISBN, b.Image, b.IsPublish, b.NumOfPages, b.Price, b.PublishDate, b.PublishYear, b.Stock, b.Summary, b.Title, b.Weight, l.LanguageName, p.PublisherName, dbo.GetAllAuthor(b.BookID) AS Authors,  dbo.GetAllTranslators(b.BookID) AS Translators, dbo.GetAllCategories(b.BookID) AS Categories FROM  dbo.BookInfo AS b INNER JOIN dbo.Languages AS l ON b.LanguageID = l.LanguageID INNER JOIN dbo.Publishers AS p ON b.PublisherID = p.PublisherID WHERE(b.[Delete] = 0)")
              .Where(b => b.BookID == id).First();
            //var BookInfo = _context.ReadAllBooks.AsNoTracking<ReadAllBook>().Where(b => b.BookID == id).First();
            return View(BookInfo);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Book=await _UW.BaseRepository<Book>().FindByIDAsync(id);
            if (Book!=null)
            {
                Book.Delete = true;
                _UW.Cmmit();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            else
            {
                var Book =await _UW.BaseRepository<Book>().FindByIDAsync(id);
                if (Book==null)
                {
                    return NotFound();
                }
                else
                {
                    var ViewModel =await (from b in _UW._Context.Books.Include(p => p.Publisher)
                                     .Include(l => l.Language)
                                     where (b.BookID == id)
                                     select new BooksCreateEditViewModel
                                     {
                                         BookID = b.BookID,
                                         Title = b.Title,
                                         ISBN = b.ISBN,
                                         NumOfPages = b.NumOfPages,
                                         Price = b.Price,
                                         Stock = b.Stock,
                                         IsPublish = (bool)b.IsPublish,
                                         LanguageID = b.LanguageID,
                                         PublisherID = b.PublisherID,
                                         Summary = b.Summary,
                                         Weight = b.Weight,
                                         RecentIsPublishe=(bool)b.IsPublish,
                                         PublisheDate=b.PublishDate,
                                       

                                     }).FirstAsync();
                    int[] AuthorArray =await (from a in _UW._Context.Author_Books
                                         where (a.BookID == id)
                                         select a.AuthorID).ToArrayAsync();
                    int[] TranstorArray=await (from t in _UW._Context.Book_Translators
                                               where(t.BookID == id)
                                               select t.TranslatorID).ToArrayAsync();
                    int[] CategoryArrray = await (from c in _UW._Context.Book_Categories
                                                  where (c.BookID == id)
                                                  select c.CategoryID).ToArrayAsync();
                    ViewModel.AuthorID = AuthorArray;
                    ViewModel.TranslatorID = TranstorArray;
                    ViewModel.CategoryID = CategoryArrray;

                    ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
                    ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
                    ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + "" + t.LastName }), "AuthorID", "NameFamily");
                    ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + "" + t.Family }), "TranslatorID", "NameFamily");
                    ViewModel.Categories = _UW.booksRepository.GetAllCategories(CategoryArrray);
                    
                    
                    
                    return View( ViewModel);

                }
            }
           

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BooksCreateEditViewModel viewModel )
        {
            if (ModelState.IsValid)
            {
                ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
                ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
                ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + "" + t.LastName }), "AuthorID", "NameFamily");
                ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + "" + t.Family }), "TranslatorID", "NameFamily");
                viewModel.Categories = _UW.booksRepository.GetAllCategories(viewModel.CategoryID);
                try
                {
                    DateTime? PublisheDate;
                    if (viewModel.IsPublish==true && viewModel.RecentIsPublishe==false)
                    {
                        PublisheDate = DateTime.Now;
                    }
                    else if (viewModel.RecentIsPublishe==true && viewModel.IsPublish==false)
                    {
                        PublisheDate = null;
                    }
                    else
                    {
                        PublisheDate = viewModel.PublisheDate;
                    }
                    Book book = new Book()
                    {
                        BookID = viewModel.BookID,
                        Title = viewModel.Title,
                        ISBN = viewModel.ISBN,
                        NumOfPages = viewModel.NumOfPages,
                        Price = viewModel.Price,
                        Stock = viewModel.Stock,
                        IsPublish = viewModel.IsPublish,
                        LanguageID = viewModel.LanguageID,
                        PublisherID = viewModel.PublisherID,
                        PublishYear = viewModel.PublishYear,
                        Summary = viewModel.Summary,
                        Weight = viewModel.Weight,
                        File = viewModel.File,
                       /// PublishDate=PublisheDate,
                         
                        Delete=false,

                    };
                    _UW.BaseRepository<Book>().Update(book);
                    var RecentAuthors = (from a in _UW._Context.Author_Books
                                         where (a.BookID == viewModel.BookID)
                                         select a.AuthorID).ToArray();
                    var RecentTranstors = (from t in _UW._Context.Book_Translators
                                           where (t.BookID == viewModel.BookID)
                                           select t.TranslatorID).ToArray();
                    var RecentCategories = (from c in _UW._Context.Book_Categories
                                            where (c.BookID == viewModel.BookID)
                                            select c.CategoryID).ToArray();
                    var DeleteAuthor = RecentAuthors.Except(viewModel.AuthorID);
                    var DeleteTranstors = RecentTranstors.Except(viewModel.TranslatorID);
                    var DeleteCategoresi = RecentCategories.Except(viewModel.CategoryID);
                    var AddAuthor = viewModel.AuthorID.Except(RecentAuthors);
                    var AddTranstors = viewModel.TranslatorID.Except(RecentTranstors);
                    var AddCategories = viewModel.CategoryID.Except(RecentCategories);
                    if (DeleteAuthor.Count() != 0)
                        _UW.BaseRepository<Author_Book>().DeleteRange(DeleteAuthor.Select(a => new Author_Book { AuthorID = a, BookID = viewModel.BookID }).ToList());
                    if (DeleteTranstors.Count() != 0)
                        _UW.BaseRepository<Book_Translator>().DeleteRange(DeleteAuthor.Select(a => new Book_Translator { TranslatorID = a, BookID = viewModel.BookID }).ToList());
                    if (DeleteCategoresi.Count() != 0)
                        _UW.BaseRepository<Book_Category>().DeleteRange(DeleteCategoresi.Select(a => new Book_Category { CategoryID = a, BookID = viewModel.BookID }).ToList());
                    if (AddAuthor.Count() != 0)
                    {
                        _UW.BaseRepository<Author_Book>().CreateRange(AddAuthor.Select(a => new Author_Book { AuthorID = a, BookID = viewModel.BookID }).ToList());
                    }
                    if (AddTranstors.Count() != 0)
                        _UW.BaseRepository<Book_Translator>().CreateRange(AddTranstors.Select(a => new Book_Translator { TranslatorID = a, BookID = viewModel.BookID }).ToList());
                    if (AddCategories.Count() != 0)
                        _UW.BaseRepository<Book_Category>().CreateRange(AddCategories.Select(a => new Book_Category { CategoryID = a, BookID = viewModel.BookID }).ToList());

                    await _UW.Cmmit();
                    ViewBag.MsgSuccess = "ذخیره اطلاعات با موفقیت انجام شد";
                    return View(viewModel);
                }
                catch (Exception)
                {
                    ViewBag.MsgError = "در ذخیره اطلاعات خطای رخ داده است.";

                    return View(viewModel);
                }
             

            }
            
            else 
            {
                ViewBag.MsgError = "اطلاعات فرم نامعتبر است";
                return View();
            }
            
        }
        public async Task<IActionResult> SearcheByIsbn(string ISBN)
        {
            if(ISBN!=null)
            {
                var book = (from b in _UW._Context.Books
                            where (b.ISBN == ISBN)
                            select new BooksIndexViewModel
                            {
                                Title = b.Title,
                                Author = BookShopContext.GetAllAuthors(b.BookID),
                                Translatorr = BookShopContext.GetAllTranstors(b.BookID),
                                category = BookShopContext.GetAllCategories(b.BookID)
                            }).FirstOrDefaultAsync();
                
                if (book.Result==null)
                {
                    ViewBag.Msq = "کتابی یا این شابک پیدا نشد";
                }
                return View(await book);
            }
            else
            {
                return View();
            }
            
            
        }
       
    }
}
