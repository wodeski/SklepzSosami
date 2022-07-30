using Microsoft.EntityFrameworkCore;
using Serwis.Models;
using Serwis.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options => // pierwsze jest nazwa schematu autentykacji nie ze nazwa cookie  
{
    options.Cookie.Name = "MyCookieAuth";//ustawienie  nazwy cookie
    options.LoginPath = "/Account/Index";//dosmylnie jest tak ustawione jesli chcemy dac w³asna sciezke wtedy trzeba ja sprecyzowac w³asnie w tym miejscu
    options.AccessDeniedPath = "/Account/AccessDenied";

});

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("AdminOnly",
    //    policy => policy.RequireClaim("Admin"));

    options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim("Department", "Orders"));

    //options.AddPolicy("HRManagerOnly",
    //    policy => policy
    //    .RequireClaim("Department", "HR")
    //    .RequireClaim("Manager").Requirements
    //    .Add(new HRManagerProbationRequirement(3)));
});

// Add services to the container.
builder.Services.AddScoped<IServiceDbContext, ServiceDbContext>();




builder.Services.AddDbContext<ServiceDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultContext")));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepository, EFRepository>(); 

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

app.UseAuthentication();//  BEZ TEGO NIE ZADZIA£A AUTORYZACJA

app.UseAuthorization();

//app.MapGet("/Service", () => "Hello World!")
//    .RequireAuthorization("AtLeast21");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
