using Microsoft.EntityFrameworkCore;
using project_1.Data;
using project_1.Dtos;
using project_1.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace project_1.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly project_1Context _db;
        public AuthorService(project_1Context db) => _db = db;

        public async Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken ct = default) =>
            await _db.Authors.AsNoTracking()
                .OrderBy(a => a.Id)
                .Select(a => a.ToDto())
                .ToListAsync(ct);

        public async Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _db.Authors.AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => a.ToDto())
                .SingleOrDefaultAsync(ct);

        public async Task<AuthorDto> CreateAsync(AuthorCreateDto input, CancellationToken ct = default)
        {
            var entity = input.ToEntity();
            _db.Authors.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, AuthorUpdateDto input, CancellationToken ct = default)
        {
            var entity = await _db.Authors.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;

            input.UpdateEntity(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.Authors.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;

            _db.Authors.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
