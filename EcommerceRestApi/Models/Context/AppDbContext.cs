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

        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<Shipment> Shipments { get; set; }

        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        public virtual DbSet<Subcategory> Subcategories { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<OrderCoupon> OrderCoupons { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<WishlistItem> WishlistItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OnlineStore;Integrated Security=True; TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Addresse__3214EC073E442168");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
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
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07C996EFAA");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07EFBD2E72");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Points).HasDefaultValue(0);

                entity.HasOne(d => d.User).WithMany(p => p.Customers)
                    .HasConstraintName("FK__Customers__UserI__5BE2A6F2");

                entity.HasMany(c => c.Wishlists)
                    .WithOne(w => w.Customer)
                    .HasForeignKey(w => w.CustomerId);

                // Change Orders to NoAction
                entity.HasMany(c => c.Orders)
                    .WithOne(o => o.Customer)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict

                // Change Invoices to NoAction
                entity.HasMany(c => c.Invoices)
                    .WithOne(i => i.Customer)
                    .HasForeignKey(i => i.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict
            });

            modelBuilder.Entity<DeliveryMethod>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Delivery__3214EC07AE1385C7");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<DeliveryMethodOrder>(entity =>
            {
                entity.HasKey(e => new { e.DeliveryMethodId, e.OrderId }).HasName("PK__Delivery__973AA5FED03E982C");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.DeliveryMethod).WithMany(p => p.DeliveryMethodOrders).HasConstraintName("FK__DeliveryM__Deliv__25518C17");

                entity.HasOne(d => d.Order).WithMany(p => p.DeliveryMethodOrders).HasConstraintName("FK__DeliveryM__Order__2645B050");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.Property(e => e.Number)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.DateOfIssue)
                    .HasColumnType("datetime");

                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.PaymentId);

                // CRITICAL: Use DeleteBehavior.NoAction or Restrict for ALL
                entity.HasOne(e => e.Customer)
                    .WithMany(e => e.Invoices)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict

                entity.HasOne(e => e.Payment)
                    .WithMany(e => e.Invoices)
                    .HasForeignKey(e => e.PaymentId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from SetNull

                entity.HasOne(e => e.Order)
                    .WithMany(e => e.Invoices)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict

                // This is fine - InvoiceItem is a child
                entity.HasMany(e => e.InvoiceItems)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__InvoiceI__3214EC07847050C4");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(e => e.Invoice)
                    .WithMany(e => e.InvoiceItems)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(e => e.InvoiceItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07C807C4CC");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValue("Pending");

                // Customer relationship - NoAction
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction)  // Changed from Restrict
                    .HasConstraintName("FK__Orders__Customer__6383C8BA");

                // OrderCoupons - CASCADE is fine (child)
                entity.HasMany(o => o.OrderCoupons)
                    .WithOne(oc => oc.Order)
                    .HasForeignKey(oc => oc.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // DeliveryMethodOrders - NoAction
                entity.HasMany(o => o.DeliveryMethodOrders)
                    .WithOne(dmo => dmo.Order)
                    .HasForeignKey(dmo => dmo.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict

                // OrderItems - NoAction
                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict

                // Payments - NoAction
                entity.HasMany(o => o.Payments)
                    .WithOne(p => p.Order)
                    .HasForeignKey(p => p.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict

                // Shipments - NoAction
                entity.HasMany(o => o.Shipments)
                    .WithOne(s => s.Order)
                    .HasForeignKey(s => s.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict

                // Invoices - NoAction
                entity.HasMany(o => o.Invoices)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from Restrict
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC07ABC16B2E");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07287FE367");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

                // Change to NoAction
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .OnDelete(DeleteBehavior.NoAction)  // Changed from SetNull
                    .HasConstraintName("FK__Payments__OrderI__75A278F5");

                // Change to NoAction
                entity.HasMany(p => p.Invoices)
                    .WithOne(i => i.Payment)
                    .HasForeignKey(i => i.PaymentId)
                    .OnDelete(DeleteBehavior.NoAction);  // Changed from SetNull
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC07B7569BC7");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07159B8D3E");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasMany(p => p.WishlistItems)
                    .WithOne(wi => wi.Product)
                    .HasForeignKey(wi => wi.ProductId);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CategoryId }).HasName("PK__ProductC__159C556D1A2729F4");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductCa__Categ__5535A963");

                entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductCa__Produ__5441852A");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC077DA90771");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Customer).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__Custome__0B91BA14");

                entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__Product__0A9D95DB");
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Shipment__3214EC079EE2232D");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.ShipmentDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.DeliveryMethod).WithMany(p => p.Shipments).HasConstraintName("FK__Shipments__Deliv__1F98B2C1");

                entity.HasOne(d => d.Order).WithMany(p => p.Shipments).HasConstraintName("FK__Shipments__Order__1EA48E88");
            });

            modelBuilder.Entity<Subcategory>(entity =>
            {
                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });


            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Coupons__3214EC07");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.MinOrderAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.MaxDiscountAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime2");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime2");

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.UsageLimit)
                    .HasDefaultValue(1);

                entity.Property(e => e.UsedCount)
                    .HasDefaultValue(0);

                entity.HasIndex(e => e.Code)
                    .IsUnique()
                    .HasDatabaseName("IX_Coupons_Code");
            });

            modelBuilder.Entity<OrderCoupon>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__OrderCoupons__3214EC07");

                entity.Property(e => e.DiscountApplied)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.AppliedAt)
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderCoupons)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderCoupons_Orders");

                entity.HasOne(e => e.Coupon)
                    .WithMany(c => c.OrderCoupons)
                    .HasForeignKey(e => e.CouponId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderCoupons_Coupons");

                entity.HasIndex(e => new { e.OrderId, e.CouponId })
                    .IsUnique()
                    .HasDatabaseName("IX_OrderCoupons_OrderCoupon");
            });

            // Wishlist Configuration
            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Wishlists__3214EC07");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasDefaultValue("Default Wishlist");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime2");

                entity.Property(e => e.IsDefault)
                    .HasDefaultValue(false);

                // Relationships
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Wishlists)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Wishlists_Customers");

                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("IX_Wishlists_CustomerId");
            });

            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__WishlistItems__3214EC07");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500);

                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                // Relationships
                entity.HasOne(e => e.Wishlist)
                    .WithMany(w => w.WishlistItems)
                    .HasForeignKey(e => e.WishlistId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WishlistItems_Wishlists");

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.WishlistItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WishlistItems_Products");

                entity.HasIndex(e => new { e.WishlistId, e.ProductId })
                    .IsUnique()
                    .HasDatabaseName("IX_WishlistItems_WishlistProduct");
            });

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
