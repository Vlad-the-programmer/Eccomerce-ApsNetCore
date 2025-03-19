using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceRestApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    About = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__3214EC078E66F864", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CountryCode = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Countrie__3214EC07C996EFAA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MethodName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Delivery__3214EC07AE1385C7", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentM__3214EC07B7569BC7", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.UserId, x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Subcategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    About = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Nip = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__3214EC07EFBD2E72", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Customers__UserI__5BE2A6F2",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    OldPrice = table.Column<int>(type: "int", nullable: true),
                    About = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LongAbout = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RatingSum = table.Column<int>(type: "int", nullable: true),
                    RatingVotes = table.Column<int>(type: "int", nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OtherPhotos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__3214EC07159B8D3E", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Subcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    Street = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    HouseNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    FlatNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Addresse__3214EC073E442168", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Addresses__Count__03F0984C",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Addresses__Custo__02FC7413",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    TotalAmount = table.Column<decimal>(type: "money", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "Pending"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__3214EC07C807C4CC", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Orders__Customer__6383C8BA",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductC__159C556D1A2729F4", x => new { x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK__ProductCa__Categ__5535A963",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ProductCa__Produ__5441852A",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "text", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reviews__3214EC077DA90771", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Reviews__Custome__0B91BA14",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Reviews__Product__0A9D95DB",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryMethodOrders",
                columns: table => new
                {
                    DeliveryMethodId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Delivery__973AA5FED03E982C", x => new { x.DeliveryMethodId, x.OrderId });
                    table.ForeignKey(
                        name: "FK__DeliveryM__Deliv__25518C17",
                        column: x => x.DeliveryMethodId,
                        principalTable: "DeliveryMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__DeliveryM__Order__2645B050",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderIte__08D097A33294AF3C", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK__OrderItem__Order__693CA210",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__OrderItem__Produ__6A30C649",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "money", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payments__3214EC07287FE367", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Payments__OrderI__75A278F5",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    DeliveryMethodId = table.Column<int>(type: "int", nullable: false),
                    ShipmentDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    EstimatedArrivalDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shipment__3214EC079EE2232D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Shipments__Deliv__1F98B2C1",
                        column: x => x.DeliveryMethodId,
                        principalTable: "DeliveryMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Shipments__Order__1EA48E88",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: false),
                    DateOfIssue = table.Column<DateTime>(type: "datetime", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoice__3214EC07E196B1F5", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Invoice__Custome__32AB8735",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Invoice__Payment__339FAB6E",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BasePricePerUnit = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TaxRate = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateUpdated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateDeleted = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__InvoiceI__3214EC07847050C4", x => x.Id);
                    table.ForeignKey(
                        name: "FK__InvoiceIt__Invoi__395884C4",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__InvoiceIt__Produ__3A4CA8FD",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "UQ__Countrie__5D9B0D2C9915823B",
                table: "Countries",
                column: "CountryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Countrie__E056F201CC4E645C",
                table: "Countries",
                column: "CountryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryMethodOrders_OrderId",
                table: "DeliveryMethodOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CustomerId",
                table: "Invoice",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_PaymentId",
                table: "Invoice",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_InvoiceId",
                table: "InvoiceItem",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_ProductId",
                table: "InvoiceItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SubcategoryId",
                table: "Products",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_DeliveryMethodId",
                table: "Shipments",
                column: "DeliveryMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OrderId",
                table: "Shipments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategories_CategoryId",
                table: "Subcategories",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "DeliveryMethodOrders");

            migrationBuilder.DropTable(
                name: "InvoiceItem");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "DeliveryMethods");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Subcategories");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
