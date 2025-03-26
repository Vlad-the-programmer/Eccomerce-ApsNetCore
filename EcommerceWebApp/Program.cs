using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Prevents JavaScript access (security)
    options.Cookie.IsEssential = true; // Ensures session is maintained
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login"; // Redirect to login if unauthorized
        options.AccessDeniedPath = "/account/access-denied";
        options.Cookie.HttpOnly = true; // Prevent JavaScript access
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Only over HTTPS
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Session expiry time
        options.SlidingExpiration = true; // Extend session if user is active
        options.Cookie.SameSite = SameSiteMode.None;
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddHttpContextAccessor(); // Register IHttpContextAccessor
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IApiService, ApiService>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
} else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

//authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
