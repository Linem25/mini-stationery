using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Stationery> Stationeries => Set<Stationery>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Stationery>(entity =>
        {
            entity.ToTable("Stationeries");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(150);
            entity.Property(s => s.Price).HasColumnType("decimal(18,2)");
            entity.Property(s => s.SupplyCode).IsRequired().HasMaxLength(20);
            entity.HasOne(s => s.Category)
                  .WithMany(c => c.Stationeries)
                  .HasForeignKey(s => s.CategoryId);
        });

        
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Bút viết" },
            new Category { Id = 2, Name = "Sổ vở" },
            new Category { Id = 3, Name = "Dụng cụ văn phòng" }
        );

        modelBuilder.Entity<Stationery>().HasData(
    new Stationery { Id = 1, SupplyCode = "VPP-0001", Name = "Bút bi Thiên Long TL-027", Price = 3500, Stock = 120, CategoryId = 1 },
    new Stationery { Id = 2, SupplyCode = "VPP-0002", Name = "Bút highlight Stabilo Boss", Price = 22000, Stock = 0, CategoryId = 1 },
    new Stationery { Id = 3, SupplyCode = "VPP-0003", Name = "Vở kẻ ngang Campus 200 trang", Price = 18000, Stock = 4, CategoryId = 2 },
    new Stationery { Id = 4, SupplyCode = "VPP-0004", Name = "Kéo văn phòng Deli", Price = 15000, Stock = 30, CategoryId = 3 }
);
    modelBuilder.Entity<Stationery>()
    .HasIndex(s => s.SupplyCode)
    .IsUnique();

modelBuilder.Entity<Stationery>()
    .Property(s => s.RowVersion)
    .IsRowVersion();

modelBuilder.Entity<Stationery>()
    .HasQueryFilter(s => !s.IsDeleted);
        
    }
}