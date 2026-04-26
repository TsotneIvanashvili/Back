using CameraShop.API.DTOs.Brand;

namespace CameraShop.API.Services.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetAllAsync();
    Task<BrandDto?> GetByIdAsync(int id);
    Task<BrandDto> CreateAsync(BrandUpsertDto dto);
    Task<BrandDto?> UpdateAsync(int id, BrandUpsertDto dto);
    Task<bool> DeleteAsync(int id);
}
