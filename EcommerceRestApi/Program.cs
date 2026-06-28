using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.Auth;
using EcommerceRestApi.Helpers.Data.DbInitializer;
using EcommerceRestApi.Helpers.Data.Permissions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<ShoppingCart>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var session = httpContextAccessor.HttpContext!.Session;
    var context = sp.GetRequiredService<AppDbContext>();

    return new ShoppingCart(context, session, httpContextAccessor);
});

//Services Configuration
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISubcategoryService, SubCategoryService>();
builder.Services.AddScoped<IReviewsService, ReviewService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IUsersManagementService, UsersManagementService>();
builder.Services.AddScoped<IWishListService, WishlistService>();
builder.Services.AddScoped<IShopCoinsService, ShopCoinsService>();
builder.Services.AddScoped<IShopCoinSettingsService, ShopCoinSettingsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IRefundService, RefundService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReturnService, ReturnService>();
builder.Services.AddScoped<IAnaliticsService, AnanliticsService>();


//authontication and authorization
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                                            .AddEntityFrameworkStores<AppDbContext>()
                                            .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "SmartScheme";
    options.DefaultAuthenticateScheme = "SmartScheme";
    options.DefaultChallengeScheme = "SmartScheme";
    options.DefaultForbidScheme = "SmartScheme";
})
.AddPolicyScheme("SmartScheme", "JWT or Cookie", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        var hasBearer = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.StartsWith("Bearer ") == true;

        if (hasBearer)
            return JwtBearerDefaults.AuthenticationScheme;

        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "user-auth";
    options.LoginPath = "/api/account/login";
    options.AccessDeniedPath = "/api/account/access-denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Session expiry time
    options.SlidingExpiration = true; // Extend session if user is active

    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = 401;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    };
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Permissions.ManageCategories, policy =>
        policy.RequireClaim("Permission", Permissions.ManageCategories));

    options.AddPolicy(Permissions.ManageUsers, policy =>
        policy.RequireClaim("Permission", Permissions.ManageUsers));

    options.AddPolicy(Permissions.ManageCoupons, policy =>
        policy.RequireClaim("Permission", Permissions.ManageCoupons));

    options.AddPolicy(Permissions.ManageOrders, policy =>
        policy.RequireClaim("Permission", Permissions.ManageOrders));

    options.AddPolicy(Permissions.ManageProduct, policy =>
        policy.RequireClaim("Permission", Permissions.ManageProduct));

    options.AddPolicy(Permissions.ManageRefunds, policy =>
        policy.RequireClaim("Permission", Permissions.ManageRefunds));
});

builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Prevents JavaScript access (security)
    options.Cookie.IsEssential = true; // Ensures session is maintained
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(AppConstants.CLIENT_URLS)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Mobile", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce API", Version = "v1" });

    // Include XML comments in Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Check if the XML file exists before including it
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        // Log a warning if the XML file is not found
        Console.WriteLine($"XML documentation file not found at: {xmlPath}");
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
        c.RoutePrefix = string.Empty;

        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });

}
else
{
    app.UseDeveloperExceptionPage();
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigins");
app.UseCors("Mobile");
app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

//Seed Database
AppDbInitializer.Seed(app);
AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.Run();
