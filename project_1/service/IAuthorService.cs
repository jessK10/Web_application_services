using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using project_1.Dtos;

namespace project_1.Services
{
    public interface IAuthorService
    {
        Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken ct = default);
        Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AuthorDto> CreateAsync(AuthorCreateDto input, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, AuthorUpdateDto input, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
