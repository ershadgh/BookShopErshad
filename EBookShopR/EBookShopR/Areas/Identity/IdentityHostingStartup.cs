﻿using System;
using BookShop.Areas.Identity.Data;
using BookShop.Models;
using EBookShopR.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BookShop.Areas.Identity.IdentityHostingStartup))]
namespace BookShop.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityDBContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityDBContextConnection")));

                //services.AddDefaultIdentity<BookShopUser>()
                //    .AddEntityFrameworkStores<IdentityDBContext>();
                services.AddIdentity<ApplicationUser, ApplicationRole>()
               /// .AddDefaultUI()
                 
                 .AddEntityFrameworkStores<IdentityDBContext>()
                 .AddErrorDescriber<ApplicationIdentityErrorDescriber>()
                 
                .AddDefaultTokenProviders();
                services.Configure<IdentityOptions>(options =>
                {
                    //Configure Password
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 7;
                    options.Password.RequiredUniqueChars = 1;
                    options.Password.RequireLowercase=false;
                    options.Password.RequireNonAlphanumeric=false;
                    options.Password.RequireUppercase = false;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = false;
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 3;

                });
            });
        }
    }
}