using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using project_1.Dtos;

namespace project_1.Services
{
    public interface IBookService
    {
        Task<IReadOnlyList<BookDto>> GetAllAsync(CancellationToken ct = default);
        Task<BookDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<BookDto> CreateAsync(BookCreateDto input, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, BookUpdateDto input, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
