using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models.Context
{
    public partial class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext()
        {
        }
        public virtual DbSet<Subcategory> Subcategories { get; set; }

        public virtual DbSet<Address> Addresses { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<DeliveryMethod> DeliveryMethods { get; set; }

        public virtual DbSet<DeliveryMethodOrder> DeliveryMethodOrders { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

        public virtual DbSet<Product> Products { get; set; }


        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<Shipment> Shipments { get; set; }

        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        // Many  to many  relationships
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Addresse__3214EC073E442168");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Country).WithMany(p => p.Addresses)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Addresses__Count__03F0984C");

                entity.HasOne(d => d.Customer).WithMany(p => p.Addresses).HasConstraintName("FK__Addresses__Custo__02FC7413");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC078E66F864");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                // ✅ Configure One-to-Many: Category -> Subcategories
                entity.HasMany(c => c.Subcategories)
                    .WithOne(s => s.Category)
                    .HasForeignKey(s => s.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ Configure Many-to-Many: Category <-> Products
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => new { pc.ProductId, pc.CategoryId });

                entity.HasOne(pc => pc.Product)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(pc => pc.ProductId)
                    .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete

                entity.HasOne(pc => pc.Category)
                    .WithMany(c => c.ProductCategories)
                    .HasForeignKey(pc => pc.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07C996EFAA");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07EFBD2E72");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Points).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.User)
                .WithMany(p => p.Customers)
                .HasForeignKey(c => c.UserId)
                .HasConstraintName("FK__Customers__UserI__5BE2A6F2")
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeliveryMethod>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Delivery__3214EC07AE1385C7");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<DeliveryMethodOrder>(entity =>
            {
                entity.HasKey(e => new { e.DeliveryMethodId, e.OrderId }).HasName("PK__Delivery__973AA5FED03E982C");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.DeliveryMethod).WithMany(p => p.DeliveryMethodOrders).HasConstraintName("FK__DeliveryM__Deliv__25518C17");

                entity.HasOne(d => d.Order).WithMany(p => p.DeliveryMethodOrders).HasConstraintName("FK__DeliveryM__Order__2645B050");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Invoice__3214EC07E196B1F5");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Invoice__Custome__32AB8735");

                entity.HasOne(d => d.Payment).WithMany(p => p.Invoices).HasConstraintName("FK__Invoice__Payment__339FAB6E");
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__InvoiceI__3214EC07847050C4");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems).HasConstraintName("FK__InvoiceIt__Invoi__395884C4");

                entity.HasOne(d => d.Product).WithMany(p => p.InvoiceItems).HasConstraintName("FK__InvoiceIt__Produ__3A4CA8FD");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07C807C4CC");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK__Orders__Customer__6383C8BA");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK__OrderIte__08D097A33294AF3C");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Order__693CA210");

                entity.HasOne(d => d.Product).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Produ__6A30C649");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07287FE367");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Ensure that OrderId is nullable in the database
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.SetNull) // SetNull behavior is correct here
                    .HasConstraintName("FK__Payments__OrderI__75A278F5");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC07B7569BC7");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07159B8D3E");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(p => p.Subcategory)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SubcategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Subcategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Subcategories");

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.DateDeleted)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // ✅ Many-to-One: Subcategory → Category
                entity.HasOne(s => s.Category)
                    .WithMany(c => c.Subcategories)
                    .HasForeignKey(s => s.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                // ✅ One-to-Many: Subcategory → Products
                entity.HasMany(s => s.Products)
                    .WithOne(p => p.Subcategory)
                    .HasForeignKey(p => p.SubcategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CategoryId }).HasName("PK__ProductC__159C556D1A2729F4");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories).HasConstraintName("FK__ProductCa__Categ__5535A963");

                entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories).HasConstraintName("FK__ProductCa__Produ__5441852A");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC077DA90771");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Customer).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__Custome__0B91BA14");

                entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__Product__0A9D95DB");
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Shipment__3214EC079EE2232D");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ShipmentDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.DeliveryMethod).WithMany(p => p.Shipments).HasConstraintName("FK__Shipments__Deliv__1F98B2C1");

                entity.HasOne(d => d.Order).WithMany(p => p.Shipments).HasConstraintName("FK__Shipments__Order__1EA48E88");
            });

            modelBuilder.Entity<ShoppingCartItem>()
               .HasOne(s => s.Product) // ShoppingCartItem has one Product
               .WithMany(p => p.ShoppingCartItems) // Product has many ShoppingCartItems
               .HasForeignKey(s => s.ProductId); // Foreign key is ProductId

            // Explicitly configure the primary key for IdentityUserLogin
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey });

            // Explicitly configure the primary key for IdentityUserRole
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new { r.UserId, r.RoleId });

            // Explicitly configure the primary key for IdentityUserClaim
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasKey(c => c.Id);

            // Explicitly configure the primary key for IdentityUserToken
            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
