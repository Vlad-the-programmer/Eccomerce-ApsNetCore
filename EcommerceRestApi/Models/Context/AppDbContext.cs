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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OnlineStore;Integrated Security=True; TrustServerCertificate=True;");

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
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Points).HasDefaultValue(0);

                entity.HasOne(d => d.User).WithMany(p => p.Customers).HasConstraintName("FK__Customers__UserI__5BE2A6F2");
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

                entity.HasOne(d => d.Customer).WithMany(p => p.Invoices).HasConstraintName("FK__Invoice__Custome__32AB8735");

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
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Status).HasDefaultValue("Pending");

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK__Orders__Customer__6383C8BA");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC07ABC16B2E");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07287FE367");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                    .OnDelete(DeleteBehavior.SetNull)
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
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CategoryId }).HasName("PK__ProductC__159C556D1A2729F4");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");

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

            modelBuilder.Entity<Subcategory>(entity =>
            {
                entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateDeleted).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.DateUpdated).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
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
