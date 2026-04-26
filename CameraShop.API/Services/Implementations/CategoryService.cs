using AutoMapper;
using CameraShop.API.DTOs.Category;
using CameraShop.API.Models;
using CameraShop.API.Repositories.Interfaces;
using CameraShop.API.Services.Interfaces;

namespace CameraShop.API.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync() =>
        _mapper.Map<IEnumerable<CategoryDto>>(await _repo.GetAllAsync());

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item is null ? null : _mapper.Map<CategoryDto>(item);
    }

    public async Task<CategoryDto> CreateAsync(CategoryUpsertDto dto)
    {
        var entity = _mapper.Map<Category>(dto);
        await _repo.AddAsync(entity);
        await _repo.SaveAsync();
        return _mapper.Map<CategoryDto>(entity);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, CategoryUpsertDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return null;
        _mapper.Map(dto, entity);
        _repo.Update(entity);
        await _repo.SaveAsync();
        return _mapper.Map<CategoryDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return false;
        _repo.Remove(entity);
        await _repo.SaveAsync();
        return true;
    }
}
