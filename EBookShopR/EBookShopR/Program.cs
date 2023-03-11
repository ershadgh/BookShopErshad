using BookShop.Models;
using BookShop.Classes;
using Microsoft.Extensions.Localization;
using ReflectionIT.Mvc.Paging;
using BookShop.Models.UnitOfWork;
using EBookShopR.Areas.Identity.Data;
using EBookShopR.Classes;
using Microsoft.AspNetCore.Identity.UI.Services;
using EBookShopR.Areas.Identity.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using EBookShopR.Areas.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IUnitOfWork,UnitOfWork>();
builder.Services.AddTransient<BookShopContext>();
builder.Services.AddTransient<BooksRepository>();
builder.Services.AddTransient<ConvertDate>();
builder.Services.AddTransient<IConvertDate, ConvertDate>();
builder.Services.AddScoped<IApplictionRoleManager,ApplictionRoleManager>();
builder.Services.AddScoped<IApplicationUserManager,ApplicationUserManager>();
builder.Services.AddScoped<ApplicationIdentityErrorDescriber>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
builder.Services.AddScoped<ISmsSender,SmsSender>();
builder.Services.AddSingleton<IAuthorizationHandler,HappyBirthDayHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, MinmumAgeHandler>();
//builder.Services.AddSingleton<IMvcActionsDiscoveryService, MvcActionsDiscoveryService>
builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
builder.Services.AddMvc();
//builder.Services.AddMvc(options =>
//{
//    var F = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
//    var L = F.Create("ModelBindingMessages", "EBookShopR");
//    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
//     (x) => L["انتخاب یکی از موارد لیست الزامی است."]);

//});
builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.LoginPath = "/Account/SignIn";
    //Options.AccessDeniedPath = "/Home/AccessDenied";
});
builder.Services.AddAuthorization(Optins =>
{
    Optins.AddPolicy("AccessToUserManager",policy => policy.RequireRole("مدیر سایت","کاربر1"));
    // Optins.AddPolicy("HappyBrithDay", policy => policy.RequireClaim(ClaimTypes.DateOfBirth, DateTime.Now.ToString("MM/dd")));
   Optins.AddPolicy("HappyBrithDay", policy => policy.Requirements.Add(new HappyBirthDayRequirement()));
    Optins.AddPolicy("Atleast18", policy => policy.Requirements.Add(new MinimumAgeRequirement(18)));
});
builder.Services.AddPaging(optins=>{
    optins.ViewName = "Bootstrap5";
    optins.HtmlIndicatorDown = "<i class='fa fa-sort-amount-down'></i>";
    optins.HtmlIndicatorUp = "<i class='fa fa-sort-amount-up'></i>";
});
builder.Services.AddSession(option =>
    {
        option.IdleTimeout = TimeSpan.FromMinutes(20);
        option.Cookie.HttpOnly = true;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy();
//app.UseAuthorization();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();
