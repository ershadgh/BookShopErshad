using BookShop.Models.ViewModels;

namespace BookShop.Models.Repository
{
    public interface IBooksRepository
    {
        List<TreeViewCategory> GetAllCategories(int[] categoryarayy);
        void BindSubCategory(TreeViewCategory category, int[] arr);
        List<BooksIndexViewModel> GetAllBook(string title, string ISBN, string Author, string Language, string Publisher, string Translator, string Catagory);

    }
}
