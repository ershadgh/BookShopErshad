namespace EBookShopR.Areas.Identity.Services
{
    public interface ISmsSender
    {
        Task<string> SendAuthSmsAsync(string Code, string PhoneNumber);
    }
}
