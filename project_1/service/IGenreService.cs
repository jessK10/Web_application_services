using project_1.Dtos;

namespace project_1.Services
{
    public interface IGenreService
    {
        Task<IReadOnlyList<GenreDto>> GetAllAsync(CancellationToken ct = default);
        Task<GenreDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<GenreDto> CreateAsync(GenreCreateDto input, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, GenreUpdateDto input, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
