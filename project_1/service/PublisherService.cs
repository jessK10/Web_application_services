using Microsoft.EntityFrameworkCore;
using project_1.Data;
using project_1.Dtos;
using project_1.Mapping;
using project_1.Models;

namespace project_1.Services
{
    public class PublisherService(project_1Context db) : IPublisherService
    {
        public async Task<IReadOnlyList<PublisherDto>> GetAllAsync(CancellationToken ct = default)
            => await db.Publishers.AsNoTracking()
                                  .OrderBy(p => p.Name)
                                  .Select(p => p.ToDto())
                                  .ToListAsync(ct);

        public async Task<PublisherDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.Publishers.AsNoTracking()
                                  .Where(p => p.Id == id)
                                  .Select(p => p.ToDto())
                                  .SingleOrDefaultAsync(ct);

        public async Task<PublisherDto> CreateAsync(PublisherCreateDto input, CancellationToken ct = default)
        {
            var entity = input.ToEntity();
            db.Publishers.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, PublisherUpdateDto input, CancellationToken ct = default)
        {
            var entity = await db.Publishers.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (entity is null) return false;

            input.UpdateEntity(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await db.Publishers.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;

            db.Publishers.Remove(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}
