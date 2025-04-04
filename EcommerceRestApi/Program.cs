using EcommerceRestApi.Helpers.Data;
using EcommerceRestApi.Helpers.Data.Auth;
using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Static;
using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using EcommerceRestApi.Services.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using EcommerceRestApi.Helpers.Cart;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});

//Services Configuration
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductsService, ProductsService>();
//builder.Services.AddScoped<IOrdersService, OrdersService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc, sc.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session));

//authontication and authorization
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                                            .AddEntityFrameworkStores<AppDbContext>()
                                            .AddDefaultTokenProviders();
builder.Services.AddAuthorization(options =>
{
    // Role-based authorization
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(UserRoles.Admin));
    options.AddPolicy("UserOnly", policy => policy.RequireRole(UserRoles.User));

    //// Policy-based authorization (e.g., require a specific claim)
    //options.AddPolicy("CanManageProducts", policy =>
    //{
    //    policy.RequireClaim("Permission", "ManageProducts");
    //});
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Default for APIs
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "user-auth";
    options.LoginPath = "/api/account/login"; // Redirect if unauthorized
    options.AccessDeniedPath = "/api/account/access-denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true; // Extend session if active
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(AppConstants.CLIENT_URLS) // Replace with your actual client URL
              .AllowCredentials() // Required for authentication cookies
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
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

//Seed Database
AppDbInitializer.Seed(app);
AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.Run();
