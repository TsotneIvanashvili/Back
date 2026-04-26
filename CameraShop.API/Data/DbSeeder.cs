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
                Email = "admin@capturecore.local",
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
                new Brand { Name = "Canon",    Country = "Japan",   FoundedYear = 1937 },
                new Brand { Name = "Nikon",    Country = "Japan",   FoundedYear = 1917 },
                new Brand { Name = "Sony",     Country = "Japan",   FoundedYear = 1946 },
                new Brand { Name = "Fujifilm", Country = "Japan",   FoundedYear = 1934 },
                new Brand { Name = "Leica",    Country = "Germany", FoundedYear = 1914 },
                new Brand { Name = "GoPro",    Country = "USA",     FoundedYear = 2002 }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "DSLR",       Description = "Digital single-lens reflex cameras." },
                new Category { Name = "Mirrorless", Description = "Compact interchangeable-lens cameras." },
                new Category { Name = "Vlogging",   Description = "Optimized for video creators." },
                new Category { Name = "Action",     Description = "Rugged action cameras." },
                new Category { Name = "Compact",    Description = "Point-and-shoot cameras." }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Cameras.AnyAsync())
        {
            var canon = await db.Brands.FirstAsync(b => b.Name == "Canon");
            var nikon = await db.Brands.FirstAsync(b => b.Name == "Nikon");
            var sony  = await db.Brands.FirstAsync(b => b.Name == "Sony");
            var fuji  = await db.Brands.FirstAsync(b => b.Name == "Fujifilm");
            var leica = await db.Brands.FirstAsync(b => b.Name == "Leica");
            var gopro = await db.Brands.FirstAsync(b => b.Name == "GoPro");

            var mirrorless = await db.Categories.FirstAsync(c => c.Name == "Mirrorless");
            var vlogging   = await db.Categories.FirstAsync(c => c.Name == "Vlogging");
            var dslr       = await db.Categories.FirstAsync(c => c.Name == "DSLR");
            var action     = await db.Categories.FirstAsync(c => c.Name == "Action");
            var compact    = await db.Categories.FirstAsync(c => c.Name == "Compact");

            // Verified Unsplash camera shots — confirmed to load, confirmed to show cameras.
            // Some are reused intentionally between similar models so every card shows a real camera.
            const string IMG = "?auto=format&fit=crop&w=1200&q=80";
            const string CAM_DSLR_HAND   = "https://images.unsplash.com/photo-1452780212940-6f5c0d14d848" + IMG; // DSLR in hand
            const string CAM_SONY_STRAP  = "https://images.unsplash.com/photo-1495707902641-75cac588d2e9" + IMG; // mirrorless w/ strap
            const string CAM_VINTAGE     = "https://images.unsplash.com/photo-1502920917128-1aa500764cbd" + IMG; // vintage body
            const string CAM_LEICA_LIKE  = "https://images.unsplash.com/photo-1500634245200-e5245c7574ef" + IMG; // rangefinder
            const string CAM_SONY_ALPHA  = "https://images.unsplash.com/photo-1516035069371-29a1b244cc32" + IMG; // Sony alpha
            const string CAM_FUJI_BOXY   = "https://images.unsplash.com/photo-1554080353-a576cf803bda" + IMG;   // Fuji X-style
            const string CAM_FUJI_X100   = "https://images.unsplash.com/photo-1581591524425-c7e0978865fc" + IMG; // X100 fixed-lens

            var cameras = new[]
            {
                new {
                    Cam = new Camera {
                        Model = "Canon EOS R6 Mark II",
                        Description = "Full-frame mirrorless built for hybrid shooters. 4K 60p uncropped, in-body stabilization, and Canon's Dual Pixel AF II.",
                        Price = 2499.99m, StockQuantity = 12, MegaPixels = 24,
                        SensorType = "Full-frame CMOS",
                        ImageUrl = CAM_SONY_STRAP,
                        BrandId = canon.Id
                    },
                    Cats = new[] { mirrorless.Id, vlogging.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Canon EOS 5D Mark IV",
                        Description = "The flagship full-frame DSLR pros have trusted for years. 30 MP, dual card slots, weather-sealed.",
                        Price = 2199.00m, StockQuantity = 5, MegaPixels = 30,
                        SensorType = "Full-frame CMOS",
                        ImageUrl = CAM_DSLR_HAND,
                        BrandId = canon.Id
                    },
                    Cats = new[] { dslr.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Nikon Z9",
                        Description = "Stacked-sensor mirrorless flagship. 8K video, 20 fps RAW, and a blackout-free EVF.",
                        Price = 5499.00m, StockQuantity = 3, MegaPixels = 45,
                        SensorType = "Full-frame Stacked CMOS",
                        ImageUrl = CAM_VINTAGE,
                        BrandId = nikon.Id
                    },
                    Cats = new[] { mirrorless.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Nikon D850",
                        Description = "High-resolution full-frame DSLR. 45.7 MP back-illuminated sensor and 4K UHD video.",
                        Price = 2999.00m, StockQuantity = 4, MegaPixels = 45,
                        SensorType = "Full-frame BSI CMOS",
                        ImageUrl = CAM_DSLR_HAND,
                        BrandId = nikon.Id
                    },
                    Cats = new[] { dslr.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Sony Alpha a7 IV",
                        Description = "The do-it-all full-frame mirrorless for photo and video creators.",
                        Price = 2799.00m, StockQuantity = 7, MegaPixels = 33,
                        SensorType = "Full-frame Exmor R",
                        ImageUrl = CAM_SONY_ALPHA,
                        BrandId = sony.Id
                    },
                    Cats = new[] { mirrorless.Id, vlogging.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Sony ZV-E1",
                        Description = "Compact full-frame vlogging camera. 4K 120p, AI-driven autofocus, product-showcase mode.",
                        Price = 2199.00m, StockQuantity = 8, MegaPixels = 12,
                        SensorType = "Full-frame Exmor R",
                        ImageUrl = CAM_SONY_STRAP,
                        BrandId = sony.Id
                    },
                    Cats = new[] { vlogging.Id, mirrorless.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Fujifilm X-T5",
                        Description = "APS-C mirrorless with retro tactile controls and a 40 MP X-Trans 5 sensor.",
                        Price = 1699.00m, StockQuantity = 9, MegaPixels = 40,
                        SensorType = "APS-C X-Trans 5",
                        ImageUrl = CAM_FUJI_BOXY,
                        BrandId = fuji.Id
                    },
                    Cats = new[] { mirrorless.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Fujifilm X100V",
                        Description = "Iconic large-sensor compact with a fixed 23mm f/2 lens and hybrid viewfinder.",
                        Price = 1399.00m, StockQuantity = 6, MegaPixels = 26,
                        SensorType = "APS-C X-Trans 4",
                        ImageUrl = CAM_FUJI_X100,
                        BrandId = fuji.Id
                    },
                    Cats = new[] { compact.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "Leica Q3",
                        Description = "Full-frame compact with a 28mm f/1.7 Summilux. 60 MP triple-resolution sensor.",
                        Price = 5995.00m, StockQuantity = 2, MegaPixels = 60,
                        SensorType = "Full-frame BSI CMOS",
                        ImageUrl = CAM_LEICA_LIKE,
                        BrandId = leica.Id
                    },
                    Cats = new[] { compact.Id }
                },
                new {
                    Cam = new Camera {
                        Model = "GoPro HERO12 Black",
                        Description = "Rugged 5.3K action camera with HyperSmooth 6.0 stabilization and waterproofing.",
                        Price = 399.00m, StockQuantity = 20, MegaPixels = 27,
                        SensorType = "1/1.9\" CMOS",
                        ImageUrl = CAM_VINTAGE,
                        BrandId = gopro.Id
                    },
                    Cats = new[] { action.Id }
                }
            };

            foreach (var c in cameras)
                db.Cameras.Add(c.Cam);
            await db.SaveChangesAsync();

            foreach (var c in cameras)
                foreach (var catId in c.Cats)
                    db.CameraCategories.Add(new CameraCategory { CameraId = c.Cam.Id, CategoryId = catId });
            await db.SaveChangesAsync();
        }
    }
}
