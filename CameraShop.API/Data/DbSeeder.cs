using CameraShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CameraShop.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@camerashop.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                Profile = new UserProfile
                {
                    FullName = "Site Administrator",
                    PhoneNumber = "+000000000",
                    Address = "HQ",
                    City = "Tbilisi",
                    Country = "Georgia"
                }
            });
            await db.SaveChangesAsync();
        }

        if (!await db.Brands.AnyAsync())
        {
            db.Brands.AddRange(
                new Brand { Name = "Canon", Country = "Japan", FoundedYear = 1937 },
                new Brand { Name = "Nikon", Country = "Japan", FoundedYear = 1917 },
                new Brand { Name = "Sony", Country = "Japan", FoundedYear = 1946 },
                new Brand { Name = "Fujifilm", Country = "Japan", FoundedYear = 1934 }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "DSLR", Description = "Digital single-lens reflex cameras." },
                new Category { Name = "Mirrorless", Description = "Compact interchangeable-lens cameras." },
                new Category { Name = "Vlogging", Description = "Optimized for video creators." },
                new Category { Name = "Action", Description = "Rugged action cameras." },
                new Category { Name = "Compact", Description = "Point-and-shoot cameras." }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Cameras.AnyAsync())
        {
            var canon = await db.Brands.FirstAsync(b => b.Name == "Canon");
            var sony = await db.Brands.FirstAsync(b => b.Name == "Sony");
            var fuji = await db.Brands.FirstAsync(b => b.Name == "Fujifilm");

            var mirrorless = await db.Categories.FirstAsync(c => c.Name == "Mirrorless");
            var vlogging = await db.Categories.FirstAsync(c => c.Name == "Vlogging");
            var dslr = await db.Categories.FirstAsync(c => c.Name == "DSLR");

            var eosR6 = new Camera
            {
                Model = "Canon EOS R6 Mark II",
                Description = "Full-frame mirrorless powerhouse for hybrid shooters.",
                Price = 2499.99m,
                StockQuantity = 12,
                MegaPixels = 24,
                SensorType = "Full-frame CMOS",
                ImageUrl = "https://picsum.photos/seed/eosr6/600/400",
                BrandId = canon.Id
            };
            var a7iv = new Camera
            {
                Model = "Sony Alpha a7 IV",
                Description = "Versatile full-frame mirrorless for stills and 4K video.",
                Price = 2799.00m,
                StockQuantity = 7,
                MegaPixels = 33,
                SensorType = "Full-frame Exmor R",
                ImageUrl = "https://picsum.photos/seed/a7iv/600/400",
                BrandId = sony.Id
            };
            var xt5 = new Camera
            {
                Model = "Fujifilm X-T5",
                Description = "APS-C mirrorless with retro controls and 40MP sensor.",
                Price = 1699.00m,
                StockQuantity = 9,
                MegaPixels = 40,
                SensorType = "APS-C X-Trans 5",
                ImageUrl = "https://picsum.photos/seed/xt5/600/400",
                BrandId = fuji.Id
            };
            db.Cameras.AddRange(eosR6, a7iv, xt5);
            await db.SaveChangesAsync();

            db.CameraCategories.AddRange(
                new CameraCategory { CameraId = eosR6.Id, CategoryId = mirrorless.Id },
                new CameraCategory { CameraId = eosR6.Id, CategoryId = vlogging.Id },
                new CameraCategory { CameraId = a7iv.Id, CategoryId = mirrorless.Id },
                new CameraCategory { CameraId = a7iv.Id, CategoryId = vlogging.Id },
                new CameraCategory { CameraId = xt5.Id, CategoryId = mirrorless.Id }
            );
            await db.SaveChangesAsync();
        }
    }
}
