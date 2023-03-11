using BookShop.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Models;
using BookShop.Models.ViewModels;

namespace BookShop.Models
{
    public class BookShopContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           // optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Server=(local);Database=EBookShopDB;Trusted_Connection=True;MultipleActiveResultSets=True");
            optionsBuilder.UseSqlServer(@"Server=(local);Database=EBookShopDB;Trusted_Connection=True;MultipleActiveResultSets=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfiguration(new Author_BookMap());
            modelBuilder.ApplyConfiguration(new CustomerMap());
            modelBuilder.ApplyConfiguration(new Order_BookMap());
            modelBuilder.ApplyConfiguration(new Book_TranslatorMap());
            modelBuilder.ApplyConfiguration(new Book_CategoryMap());
            modelBuilder.Entity<ReadAllBook>().ToView("ReadAllBooks").HasNoKey();
            modelBuilder.Entity<Book>().HasQueryFilter(b => (bool)!b.Delete);
            modelBuilder.Entity<Book>().Property(b => b.Delete).HasDefaultValueSql("0");
            ///  modelBuilder.Entity<Book>().Property(b => b.PublishDate).HasDefaultValueSql("select Convert(datetime,GetDate())");
              
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Provice> Provices { get; set; }
        public DbSet<Author_Book> Author_Books { get; set; }
        public DbSet<Order_Book> Order_Books { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Translator> Translators { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Book_Category> Book_Categories { get; set; }
        public DbSet<Book_Translator> Book_Translators { get; set; }
        public DbSet<ReadAllBook> ReadAllBooks { get; set; }
        [DbFunction("GetAllAuthor","dbo")]
        public static string GetAllAuthors(int BookID)
        {
           throw new NotImplementedException();
        }
        [DbFunction("GetAllTranslators","dbo")]
        public static string GetAllTranstors(int BookID0)
        {
            throw new NotImplementedException();
        }
        [DbFunction("GetAllCategories", "dbo")]
        public static string GetAllCategories(int BookID)
        {
            throw new NotImplementedException();
        }
    }
}
