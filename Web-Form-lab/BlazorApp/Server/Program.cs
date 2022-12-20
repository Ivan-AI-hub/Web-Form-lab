using BlazorApp.Server.Data;
using MediatR;
using BlazorApp.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Whosales.Application.Interfaces;
using Whosales.Persistence;
using Whosales.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var userConnectionString = builder.Configuration.GetConnectionString("UserConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(userConnectionString));

var wholesaleConnectionString = builder.Configuration.GetConnectionString("WholesaleConnection");
builder.Services.AddDbContext<WholesaleContext>(options =>
    options.UseSqlServer(wholesaleConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddTransient<IWholesaleContext, WholesaleContext>();
builder.Services.AddMediatR(typeof(IWholesaleContext).Assembly);
builder.Services.AddTransient<ProductService>();
builder.Services.AddTransient<CustomerService>();
builder.Services.AddTransient<ReleaseReportService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
