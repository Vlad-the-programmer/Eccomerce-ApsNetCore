using EcommerceWebApp.ApiServices;
using EcommerceWebApp.AppGlobals;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.AppViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

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
        options.Cookie.Name = "user-auth";
        options.LoginPath = "/account/login"; // Redirect to login if unauthorized
        options.AccessDeniedPath = "/account/access-denied";
        options.Cookie.HttpOnly = true; // Prevent JavaScript access
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Only over HTTPS
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Session expiry time
        options.SlidingExpiration = true; // Extend session if user is active
        options.Cookie.SameSite = SameSiteMode.Lax;
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

app.Use(async (context, next) =>
{
    var client = new HttpClient(new HttpClientHandler { UseCookies = true });
    client.BaseAddress = new Uri(AppConstants.BASE_URL);

    var response = await client.GetAsync(GlobalConstants.GetCurrentUserEndpoint);
    if (response.IsSuccessStatusCode)
    {
        var userJson = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<CurrentUserViewModel>(userJson);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        context.User = new ClaimsPrincipal(identity);
    }

    await next();
});


app.Run();
