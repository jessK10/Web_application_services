using project_1.Dtos;

namespace project_1.Services
{
    public interface IMemberService
    {
        Task<IReadOnlyList<MemberDto>> GetAllAsync(CancellationToken ct = default);
        Task<MemberDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<MemberDto> CreateAsync(MemberCreateDto input, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, MemberUpdateDto input, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
