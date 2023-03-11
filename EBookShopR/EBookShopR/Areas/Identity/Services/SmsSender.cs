using System.Net;

namespace EBookShopR.Areas.Identity.Services
{
    public class SmsSender: ISmsSender
    {
        public async Task<string> SendAuthSmsAsync(string Code,string PhoneNumber)
        {
            HttpClient httpclient = new HttpClient();
            var httpreponse = await httpclient.GetAsync($"https://api.kavenegar.com/v1/6167303731702B6A4A6776654A6D65394A674D624C36426F4E6A38453556784977587A364D4C46766562673D/verify/lookup.json?receptor={PhoneNumber}&token={Code}&template=AuthVerify");
            if (httpreponse.StatusCode == HttpStatusCode.OK)
                return "success";
            else
                return "failed";
        }
    }
}
