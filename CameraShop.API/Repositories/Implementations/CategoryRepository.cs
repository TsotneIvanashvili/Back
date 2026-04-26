using CameraShop.API.Data;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;

namespace CameraShop.API.Repositories.Implementations;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }
}
