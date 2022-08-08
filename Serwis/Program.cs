using Microsoft.EntityFrameworkCore;
using Serwis.Models;
using Serwis.Repository;
using Serwis.Repository.AccountAuth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options => // pierwsze jest nazwa schematu autentykacji nie ze nazwa cookie  
{
    options.Cookie.Name = "MyCookieAuth";//ustawienie  nazwy cookie
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
    //options.AddPolicy("HRManagerOnly",
    //    policy => policy
    //    .RequireClaim("Department", "HR")
    //    .RequireClaim("Manager").Requirements
    //    .Add(new HRManagerProbationRequirement(3)));
});

// Add services to the container.
builder.Services.AddScoped<IServiceDbContext, ServiceDbContext>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // do obs³ugi sesji po stronie view w tym wypadku oczywiscie ma tez inne zastosowanie 


builder.Services.AddDbContext<ServiceDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultContext")));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepository, EFRepository>();
builder.Services.AddScoped<AccountAuthRepository, AccountAuthRepository>();
builder.Services.AddScoped<EmailSender, EmailSender>();
builder.Services.AddTransient<ReportRepository, ReportRepository>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); // POZWALA NA WIDZENIE ZMIAN PO ODSWIEZENIU STRONY
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(
    options =>
{
   // options.IdleTimeout = TimeSpan.FromSeconds(10);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
}
);

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
