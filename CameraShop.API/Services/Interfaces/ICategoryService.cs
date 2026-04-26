using CameraShop.API.DTOs.Category;

namespace CameraShop.API.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(CategoryUpsertDto dto);
    Task<CategoryDto?> UpdateAsync(int id, CategoryUpsertDto dto);
    Task<bool> DeleteAsync(int id);
}
