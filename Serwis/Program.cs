using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Core.Service;
using Serwis.Models;
using Serwis.Models.Domains;
using Serwis.Models.ViewModels;
using Serwis.Persistance;
using Serwis.Persistance.Repository;
using Serwis.Persistance.Service;
using Serwis.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options => // pierwsze jest nazwa schematu autentykacji nie ze nazwa cookie  
{
    options.Cookie.Name = "CookieAuth";//ustawienie  nazwy cookie
    options.LoginPath = "/Account/Login";//dosmylnie jest tak ustawione jesli chcemy dac w³asna sciezke wtedy trzeba ja sprecyzowac w³asnie w tym miejscu
    options.AccessDeniedPath = "/Home/Index";

});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User",
        policy => policy
        .RequireClaim("User"));

    options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim("Admin"));
});

builder.Services.AddTransient<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // do obs³ugi sesji po stronie view w tym wypadku oczywiscie ma tez inne zastosowanie 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultContext")));
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IService, Service>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<AuthRepository, AuthRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IOrderPositionRepository, OrderPositionRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddScoped<EmailSender, EmailSender>();
builder.Services.AddScoped<GenarateHtmlEmail, GenarateHtmlEmail>();
builder.Services.AddTransient<ReportRepository, ReportRepository>();
builder.Services.AddSingleton<List<OrderPositionViewModel>, List<OrderPositionViewModel>>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); // POZWALA NA WIDZENIE ZMIAN PO ODSWIEZENIU STRONY
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

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

app.UseSession(); // dodanie midleware

app.UseRouting();

app.UseAuthentication();//  BEZ TEGO NIE ZADZIA£A AUTORYZACJA

app.UseAuthorization();



//app.MapGet("/Service", () => "Hello World!")
//    .RequireAuthorization("AtLeast21");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
