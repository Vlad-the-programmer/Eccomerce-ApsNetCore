using EcommerceWebApp.ApiServices;
using EcommerceWebApp.AppGlobals;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<CookieContainer>();


builder.Services.AddHttpClient<IApiService, ApiService>()
    .ConfigureHttpClient((provider, client) =>
    {
        client.BaseAddress = new Uri(AppConstants.RESTAPI_BASE_URL);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("X-Client-Type", "web");
    })
    .ConfigurePrimaryHttpMessageHandler((provider) =>
    {
        return new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = provider.GetRequiredService<CookieContainer>(),
            AllowAutoRedirect = true,
            UseDefaultCredentials = false
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

//app.Use(async (context, next) =>
//{
//    try
//    {

//        // Skip if user is already authenticated
//        if (context.User.Identity.IsAuthenticated)
//        {
//            await next(context);
//            return;
//        }

//        var session = context.Session;
//        var userJson = session.GetString("CurrentUser");

//        if (!string.IsNullOrEmpty(userJson))
//        {
//            try
//            {
//                var user = JsonSerializer.Deserialize<CurrentUserDTO>(userJson, GlobalConstants.JsonSerializerOptions);

//                if (user != null && !string.IsNullOrEmpty(user.UserId))
//                {
//                    var claims = new List<Claim>
//                    {
//                        new Claim(ClaimTypes.NameIdentifier, user.UserId),
//                        new Claim(ClaimTypes.Name, user.FullName ?? "User"),
//                        new Claim(ClaimTypes.Email, user.Email ?? ""),
//                        new Claim("UserName", user.UserName ?? ""),
//                        new Claim("CustomerId", user.CustomerId?.ToString() ?? ""),
//                        new Claim(ClaimTypes.Role, user.Role ?? "User")
//                    };

//                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//                    context.User = new ClaimsPrincipal(identity);

//                    //await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, context.User);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[Middleware] Deserialization error: {ex.Message}");
//                session.Remove("CurrentUser"); // Clear invalid data
//            }
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"[Middleware] Exception: {ex.Message}");
//    }

//    await next(context);
//});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
