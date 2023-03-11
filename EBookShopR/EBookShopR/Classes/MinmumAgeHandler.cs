using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EBookShopR.Classes
{
    public class MinmumAgeHandler:AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
           if(!context.User.HasClaim(c=>c.Type==ClaimTypes.DateOfBirth))
            {
                return Task.CompletedTask;
            }
            var DateOfBirthDay = Convert.ToDateTime(context.User.FindFirstValue(ClaimTypes.DateOfBirth));
            int Age = DateTime.Today.Year - DateOfBirthDay.Year;
            if(DateOfBirthDay>DateTime.Today.AddYears(-Age))
            {
                Age--;
            }
            if (Age>=requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
