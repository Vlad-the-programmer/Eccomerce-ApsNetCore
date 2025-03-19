using EcommerceRestApi.Helpers.Data;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
//builder.Services.AddScoped<ISuppliersService, SuppliersService>();
//builder.Services.AddScoped<IBrandsService, BrandsService>();
//builder.Services.AddScoped<ICompaniesService, CompaniesService>();
//builder.Services.AddScoped<IProductsService, ProductsService>();
//builder.Services.AddScoped<IOrdersService, OrdersService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped(sc => ShoppingCart.GetShoppingCart(sc));

//authontication and authorization
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        #if DEBUG
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
        #else
            c.SwaggerEndpoint("/MyApp/swagger/v1/swagger.json", "MyAPI");
        #endif
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


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

//Seed Database
AppDbInitializer.Seed(app);
AppDbInitializer.SeedUsersAndRolesAsync(app).Wait();

app.Run();
