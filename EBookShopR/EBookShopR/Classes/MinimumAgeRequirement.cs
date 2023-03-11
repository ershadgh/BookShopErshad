﻿using Microsoft.AspNetCore.Authorization;

namespace EBookShopR.Classes
{
    public class MinimumAgeRequirement:IAuthorizationRequirement
    {
        public int MinimumAge { get; set; }
        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }
}
