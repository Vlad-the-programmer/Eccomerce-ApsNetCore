using EcommerceWebApp.ApiServices;
using EcommerceWebApp.AppGlobals;
using EcommerceWebApp.Helpers.Cart;
using EcommerceWebApp.Models.AppViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ShoppingCart>(sc =>
    ShoppingCart.GetShoppingCart(sc,
            sc.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session));

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
        options.Cookie.SameSite = SameSiteMode.None;
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<CookieContainer>();
builder.Services.AddHttpClient<IApiService, ApiService>()
        .ConfigureHttpClient((provider, client) =>
        {
            client.BaseAddress = new Uri(AppConstants.BASE_URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
        .ConfigurePrimaryHttpMessageHandler(provider =>
        {
            return new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = provider.GetRequiredService<CookieContainer>(),
                AllowAutoRedirect = true
            };
        });



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
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
app.Use(async (context, next) =>
{
    try
    {
        var session = context.Session;
        var userJson = session.GetString("CurrentUser");

        CurrentUserViewModel? user = null;

        if (string.IsNullOrEmpty(userJson))
        {
            // Resolve IApiService from DI container
            var apiService = context.RequestServices.GetRequiredService<IApiService>();

            var responseJson = await apiService.GetDataAsync(GlobalConstants.GetCurrentUserEndpoint);

            if (!string.IsNullOrEmpty(responseJson))
            {
                userJson = responseJson;
                session.SetString("CurrentUser", userJson);
            }
        }

        // Deserialize from cached string (whether newly fetched or already in session)
        if (!string.IsNullOrEmpty(userJson))
        {
            user = JsonSerializer.Deserialize<CurrentUserViewModel>(userJson, GlobalConstants.JsonSerializerOptions);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName ?? "Guest"),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("UserName", user.UserName ?? ""),  // Custom claim for username
                    new Claim("CustomerId", user.CustomerId?.ToString() ?? "")
                };

                if (user.IsAdmin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                context.User = new ClaimsPrincipal(identity);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Middleware] Exception: {ex.Message}");
    }

    await next(context);
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
