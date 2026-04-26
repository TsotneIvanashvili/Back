using AutoMapper;
using CameraShop.API.DTOs.Camera;
using CameraShop.API.Helpers;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using CameraShop.API.Services.Interfaces;

namespace CameraShop.API.Services.Implementations;

public class CameraService : ICameraService
{
    private readonly ICameraRepository _cameras;
    private readonly IBrandRepository _brands;
    private readonly ICategoryRepository _categories;
    private readonly IMapper _mapper;

    public CameraService(
        ICameraRepository cameras,
        IBrandRepository brands,
        ICategoryRepository categories,
        IMapper mapper)
    {
        _cameras = cameras;
        _brands = brands;
        _categories = categories;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CameraDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<CameraDto>>(await _cameras.GetAllWithDetailsAsync());

    public async Task<CameraDto?> GetByIdAsync(int id)
    {
        var camera = await _cameras.GetByIdWithDetailsAsync(id);
        return camera is null ? null : _mapper.Map<CameraDto>(camera);
    }

    public async Task<CameraDto> CreateAsync(CameraUpsertDto dto)
    {
        if (await _brands.GetByIdAsync(dto.BrandId) is null)
            throw new AppException($"Brand {dto.BrandId} does not exist.", 404);

        var camera = _mapper.Map<Camera>(dto);
        await _cameras.AddAsync(camera);
        await _cameras.SaveAsync();

        if (dto.CategoryIds.Any())
        {
            await _cameras.ReplaceCategoriesAsync(camera, dto.CategoryIds);
            await _cameras.SaveAsync();
        }

        var refreshed = await _cameras.GetByIdWithDetailsAsync(camera.Id);
        return _mapper.Map<CameraDto>(refreshed!);
    }

    public async Task<CameraDto?> UpdateAsync(int id, CameraUpsertDto dto)
    {
        var camera = await _cameras.GetByIdWithDetailsAsync(id);
        if (camera is null) return null;

        if (await _brands.GetByIdAsync(dto.BrandId) is null)
            throw new AppException($"Brand {dto.BrandId} does not exist.", 404);

        camera.Model = dto.Model;
        camera.Description = dto.Description;
        camera.Price = dto.Price;
        camera.StockQuantity = dto.StockQuantity;
        camera.MegaPixels = dto.MegaPixels;
        camera.SensorType = dto.SensorType;
        camera.ImageUrl = dto.ImageUrl;
        camera.BrandId = dto.BrandId;

        _cameras.Update(camera);
        await _cameras.ReplaceCategoriesAsync(camera, dto.CategoryIds);
        await _cameras.SaveAsync();

        var refreshed = await _cameras.GetByIdWithDetailsAsync(camera.Id);
        return _mapper.Map<CameraDto>(refreshed!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var camera = await _cameras.GetByIdAsync(id);
        if (camera is null) return false;
        _cameras.Remove(camera);
        await _cameras.SaveAsync();
        return true;
    }
}
