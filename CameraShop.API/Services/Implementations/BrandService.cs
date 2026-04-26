using AutoMapper;
using CameraShop.API.DTOs.Brand;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using CameraShop.API.Services.Interfaces;

namespace CameraShop.API.Services.Implementations;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repo;
    private readonly IMapper _mapper;

    public BrandService(IBrandRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BrandDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<BrandDto>>(await _repo.GetAllAsync());

    public async Task<BrandDto?> GetByIdAsync(int id)
    {
        var brand = await _repo.GetByIdAsync(id);
        return brand is null ? null : _mapper.Map<BrandDto>(brand);
    }

    public async Task<BrandDto> CreateAsync(BrandUpsertDto dto)
    {
        var brand = _mapper.Map<Brand>(dto);
        await _repo.AddAsync(brand);
        await _repo.SaveAsync();
        return _mapper.Map<BrandDto>(brand);
    }

    public async Task<BrandDto?> UpdateAsync(int id, BrandUpsertDto dto)
    {
        var brand = await _repo.GetByIdAsync(id);
        if (brand is null) return null;

        _mapper.Map(dto, brand);
        _repo.Update(brand);
        await _repo.SaveAsync();
        return _mapper.Map<BrandDto>(brand);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var brand = await _repo.GetByIdAsync(id);
        if (brand is null) return false;
        _repo.Remove(brand);
        await _repo.SaveAsync();
        return true;
    }
}
