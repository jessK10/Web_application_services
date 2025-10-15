using Microsoft.EntityFrameworkCore;
using project_1.Data;
using project_1.Dtos;
using project_1.Mapping;
using project_1.Models;

namespace project_1.Services
{
    public class GenreService(project_1Context db) : IGenreService
    {
        public async Task<IReadOnlyList<GenreDto>> GetAllAsync(CancellationToken ct = default)
            => await db.Genres.AsNoTracking()
                              .OrderBy(g => g.Name)
                              .Select(g => g.ToDto())
                              .ToListAsync(ct);

        public async Task<GenreDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await db.Genres.AsNoTracking()
                              .Where(g => g.Id == id)
                              .Select(g => g.ToDto())
                              .SingleOrDefaultAsync(ct);

        public async Task<GenreDto> CreateAsync(GenreCreateDto input, CancellationToken ct = default)
        {
            var entity = input.ToEntity();
            db.Genres.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, GenreUpdateDto input, CancellationToken ct = default)
        {
            var entity = await db.Genres.FirstOrDefaultAsync(g => g.Id == id, ct);
            if (entity is null) return false;

            input.UpdateEntity(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await db.Genres.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;

            db.Genres.Remove(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}
