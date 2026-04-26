using CameraShop.API.Data;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;

namespace CameraShop.API.Repositories.Implementations;

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
    public BrandRepository(AppDbContext context) : base(context) { }
}
