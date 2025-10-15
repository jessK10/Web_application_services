using project_1.Dtos;

namespace project_1.Services
{
    public interface IPublisherService
    {
        Task<IReadOnlyList<PublisherDto>> GetAllAsync(CancellationToken ct = default);
        Task<PublisherDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PublisherDto> CreateAsync(PublisherCreateDto input, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, PublisherUpdateDto input, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
