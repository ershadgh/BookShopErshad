using EBookShopR.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EBookShopR.Models
{
    public class GoogleReCapchaModelBase
    {
        [GoogleRecapchaValidationAttribute]
        [BindProperty(Name = "g-recaptcha-response")]
        public string GoogleReCaptchaResponse { get; set; }
    }
}
