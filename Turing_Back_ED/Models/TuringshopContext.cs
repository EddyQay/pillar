using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Turing_Back_ED.Models
{
    public partial class TuringshopContext : DbContext
    {
        public TuringshopContext()
        {
        }

        public TuringshopContext(DbContextOptions<TuringshopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attribute> Attributes { get; set; }
        public virtual DbSet<AttributeValue> AttributeValues { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Review_> Reviews { get; set; }
        public virtual DbSet<Shipping> Shippings { get; set; }
        public virtual DbSet<ShippingRegion> ShippingRegions { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Tax> Taxes { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Attribute>(entity =>
            {
                entity.ToTable("attribute", "turingshop");

                entity.Property(e => e.AttributeId)
                    .HasColumnName("attribute_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AttributeValue>(entity =>
            {
                entity.ToTable("attribute_value", "turingshop");

                entity.HasIndex(e => e.AttributeId)
                    .HasName("idx_attribute_value_attribute_id");

                entity.Property(e => e.AttributeValueId)
                    .HasColumnName("attribute_value_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AttributeId)
                    .HasColumnName("attribute_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Audit>(entity =>
            {
                entity.ToTable("audit", "turingshop");

                entity.HasIndex(e => e.OrderId)
                    .HasName("idx_audit_order_id");

                entity.Property(e => e.AuditId)
                    .HasColumnName("audit_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedOn).HasColumnName("created_on");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .IsUnicode(false);

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category", "turingshop");

                entity.HasIndex(e => e.DepartmentId)
                    .HasName("idx_category_department_id");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Added).HasColumnName("added");

                entity.Property(e => e.DepartmentId)
                    .HasColumnName("department_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Modified).HasColumnName("modified");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer", "turingshop");

                entity.HasIndex(e => e.Email)
                    .HasName("idx_customer_email")
                    .IsUnique();

                entity.HasIndex(e => e.ShippingRegionId)
                    .HasName("idx_customer_shipping_region_id");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("customer_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Added).HasColumnName("added");

                entity.Property(e => e.Address1)
                    .HasColumnName("address_1")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Address2)
                    .HasColumnName("address_2")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreditCard)
                    .HasColumnName("credit_card")
                    .IsUnicode(false);

                entity.Property(e => e.DayPhone)
                    .HasColumnName("day_phone")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EvePhone)
                    .HasColumnName("eve_phone")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MobPhone)
                    .HasColumnName("mob_phone")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Modified).HasColumnName("modified");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(131)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasColumnName("postal_code")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ShippingRegionId)
                    .HasColumnName("shipping_region_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department", "turingshop");

                entity.Property(e => e.DepartmentId)
                    .HasColumnName("department_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Added).HasColumnName("added");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Modified).HasColumnName("modified");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.ToTable("order_detail", "turingshop");

                entity.HasIndex(e => e.OrderId)
                    .HasName("idx_order_detail_order_id");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Attributes)
                    .IsRequired()
                    .HasColumnName("attributes")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("product_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UnitCost)
                    .HasColumnName("unit_cost")
                    .HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.ToTable("orders", "turingshop");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("idx_orders_customer_id");

                entity.HasIndex(e => e.ShippingId)
                    .HasName("idx_orders_shipping_id");

                entity.HasIndex(e => e.TaxId)
                    .HasName("idx_orders_tax_id");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AuthCode)
                    .HasColumnName("auth_code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnName("created_on");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("customer_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShippedOn).HasColumnName("shipped_on");

                entity.Property(e => e.ShippingId)
                    .HasColumnName("shipping_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.TaxId)
                    .HasColumnName("tax_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TotalAmount)
                    .HasColumnName("total_amount")
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("0.00");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product", "turingshop");

                entity.HasIndex(e => new { e.Name, e.Description })
                    .HasName("idx_ft_product_name_description");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Added).HasColumnName("added");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.DiscountedPrice)
                    .HasColumnName("discounted_price")
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValueSql("0.00");

                entity.Property(e => e.Display)
                    .HasColumnName("display")
                    .HasColumnType("smallint(6)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Image2)
                    .HasColumnName("image_2")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Modified).HasColumnName("modified");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.AttributeValueId });

                entity.ToTable("product_attribute", "turingshop");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AttributeValueId)
                    .HasColumnName("attribute_value_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CategoryId });

                entity.ToTable("product_category", "turingshop");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Review_>(entity =>
            {
                entity.ToTable("review", "turingshop");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("idx_review_customer_id");

                entity.HasIndex(e => e.ProductId)
                    .HasName("idx_review_product_id");

                entity.Property(e => e.ReviewId)
                    .HasColumnName("review_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedOn).HasColumnName("created_on");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("customer_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Modified).HasColumnName("modified");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .HasColumnType("smallint(6)");

                entity.Property(e => e.Review)
                    .IsRequired()
                    .HasColumnName("review")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Shipping>(entity =>
            {
                entity.ToTable("shipping", "turingshop");

                entity.HasIndex(e => e.ShippingRegionId)
                    .HasName("idx_shipping_shipping_region_id");

                entity.Property(e => e.ShippingId)
                    .HasColumnName("shipping_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ShippingCost)
                    .HasColumnName("shipping_cost")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.ShippingRegionId)
                    .HasColumnName("shipping_region_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ShippingType)
                    .IsRequired()
                    .HasColumnName("shipping_type")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShippingRegion>(entity =>
            {
                entity.ToTable("shipping_region", "turingshop");

                entity.Property(e => e.ShippingRegionId)
                    .HasColumnName("shipping_region_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ShippingRegion1)
                    .IsRequired()
                    .HasColumnName("shipping_region")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.ToTable("shopping_cart", "turingshop");

                entity.HasIndex(e => e.CartId)
                    .HasName("idx_shopping_cart_cart_id");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Added).HasColumnName("added_on");

                entity.Property(e => e.Attributes)
                    .IsRequired()
                    .HasColumnName("attributes")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.BuyNow)
                    .HasColumnName("buy_now")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.CartId)
                    .IsRequired()
                    .HasColumnName("cart_id")
                    .HasColumnType("char(32)");

                entity.Property(e => e.Modified).HasColumnName("modified");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Tax>(entity =>
            {
                entity.ToTable("tax", "turingshop");

                entity.Property(e => e.TaxId)
                    .HasColumnName("tax_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TaxPercentage)
                    .HasColumnName("tax_percentage")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.TaxType)
                    .IsRequired()
                    .HasColumnName("tax_type")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }
    }
}
