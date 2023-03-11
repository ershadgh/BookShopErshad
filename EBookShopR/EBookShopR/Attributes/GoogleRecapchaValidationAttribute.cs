using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace EBookShopR.Attributes
{
    public class GoogleRecapchaValidationAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            Lazy<ValidationResult> errorResult = new Lazy<ValidationResult>(() => new ValidationResult("لطفا با زدن تیک من رباط نیستم هویت خود را تایید کنید",new string[] {validationContext.MemberName}));
            if (value==null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return errorResult.Value;
            }
           // return base.IsValid(value, validationContext);
            IConfiguration configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            string reCaptchResponse = value.ToString();
            string reCaptchaSecret = configuration.GetValue<string>("GoogleReCaptcha:SecretKey");

            HttpClient httpClient = new HttpClient();
            var httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={reCaptchResponse}").Result;
            if (httpResponse.StatusCode!=HttpStatusCode.OK)
            {
                return errorResult.Value;
            }
            string JsonResoponse=httpResponse.Content.ReadAsStringAsync().Result;
            dynamic jsonData=JObject.Parse(JsonResoponse);
            if (jsonData.success!= true.ToString().ToLower())
            {
                return errorResult.Value;
               
            }
            return ValidationResult.Success;

        }
    }
}
