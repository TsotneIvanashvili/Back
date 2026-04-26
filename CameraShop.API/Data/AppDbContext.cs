using CameraShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CameraShop.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Camera> Cameras => Set<Camera>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CameraCategory> CameraCategories => Set<CameraCategory>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---- User ----
        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => u.Username).IsUnique();
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Username).IsRequired().HasMaxLength(60);
            b.Property(u => u.Email).IsRequired().HasMaxLength(120);
            b.Property(u => u.PasswordHash).IsRequired();
            b.Property(u => u.Role).IsRequired().HasMaxLength(20);
        });

        // ---- One-to-One: User <-> UserProfile ----
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---- One-to-Many: Brand -> Cameras ----
        modelBuilder.Entity<Brand>()
            .HasMany(b => b.Cameras)
            .WithOne(c => c.Brand)
            .HasForeignKey(c => c.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Brand>().Property(b => b.Name).IsRequired().HasMaxLength(80);
        modelBuilder.Entity<Brand>().HasIndex(b => b.Name).IsUnique();

        // ---- Camera ----
        modelBuilder.Entity<Camera>(b =>
        {
            b.Property(c => c.Model).IsRequired().HasMaxLength(120);
            b.Property(c => c.Price).HasColumnType("decimal(10,2)");
        });

        // ---- Many-to-Many: Camera <-> Category via CameraCategory ----
        modelBuilder.Entity<CameraCategory>()
            .HasKey(cc => new { cc.CameraId, cc.CategoryId });

        modelBuilder.Entity<CameraCategory>()
            .HasOne(cc => cc.Camera)
            .WithMany(c => c.CameraCategories)
            .HasForeignKey(cc => cc.CameraId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CameraCategory>()
            .HasOne(cc => cc.Category)
            .WithMany(c => c.CameraCategories)
            .HasForeignKey(cc => cc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(60);
        modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

        // ---- One-to-Many: User -> Orders ----
        modelBuilder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---- One-to-Many: Order -> OrderItems ----
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<OrderItem>().Property(o => o.UnitPrice).HasColumnType("decimal(10,2)");

        // ---- OrderItem -> Camera (many OrderItems can refer to the same Camera) ----
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Camera)
            .WithMany(c => c.OrderItems)
            .HasForeignKey(oi => oi.CameraId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- One-to-One: User <-> Cart ----
        modelBuilder.Entity<User>()
            .HasOne(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---- One-to-Many: Cart -> CartItems ----
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // CartItem -> Camera (don't allow deleting a camera that's in someone's cart)
        modelBuilder.Entity<CartItem>()
            .HasOne(i => i.Camera)
            .WithMany()
            .HasForeignKey(i => i.CameraId)
            .OnDelete(DeleteBehavior.Restrict);

        // One row per (cart, camera) — prevents duplicate lines
        modelBuilder.Entity<CartItem>()
            .HasIndex(i => new { i.CartId, i.CameraId })
            .IsUnique();
    }
}
