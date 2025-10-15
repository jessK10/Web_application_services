using Microsoft.EntityFrameworkCore;
using project_1.Data;
using project_1.Dtos;
using project_1.Mapping;
using project_1.Models;

namespace project_1.Services
{
    public class MemberService(project_1Context db) : IMemberService
    {
        public async Task<IReadOnlyList<MemberDto>> GetAllAsync(CancellationToken ct = default)
            => await db.Members.AsNoTracking()
                               .OrderBy(m => m.Name)
                               .Select(m => m.ToDto())
                               .ToListAsync(ct);

        public async Task<MemberDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.Members.AsNoTracking()
                               .Where(m => m.Id == id)
                               .Select(m => m.ToDto())
                               .SingleOrDefaultAsync(ct);

        public async Task<MemberDto> CreateAsync(MemberCreateDto input, CancellationToken ct = default)
        {
            var entity = input.ToEntity();
            db.Members.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, MemberUpdateDto input, CancellationToken ct = default)
        {
            var entity = await db.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (entity is null) return false;

            input.UpdateEntity(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await db.Members.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;

            db.Members.Remove(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}
