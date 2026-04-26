using CameraShop.API.DTOs.Camera;

namespace CameraShop.API.Services.Interfaces;

public interface ICameraService
{
    Task<IEnumerable<CameraDto>> GetAllAsync();
    Task<CameraDto?> GetByIdAsync(int id);
    Task<CameraDto> CreateAsync(CameraUpsertDto dto);
    Task<CameraDto?> UpdateAsync(int id, CameraUpsertDto dto);
    Task<bool> DeleteAsync(int id);
}
