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
    public class BookService : IBookService
    {
        private readonly project_1Context _db;
        public BookService(project_1Context db) => _db = db;

        public async Task<IReadOnlyList<BookDto>> GetAllAsync(CancellationToken ct = default) =>
            await _db.Books.AsNoTracking()
                .OrderBy(b => b.Id)
                .Select(b => new BookDto(
                    b.Id, b.Title, b.Year,
                    new AuthorSummaryDto(b.AuthorId, b.Author!.Name)))
                .ToListAsync(ct);

        public async Task<BookDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _db.Books.AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BookDto(
                    b.Id, b.Title, b.Year,
                    new AuthorSummaryDto(b.AuthorId, b.Author!.Name)))
                .SingleOrDefaultAsync(ct);

        public async Task<BookDto> CreateAsync(BookCreateDto input, CancellationToken ct = default)
        {
            var authorExists = await _db.Authors.AnyAsync(a => a.Id == input.AuthorId, ct);
            if (!authorExists)
                throw new KeyNotFoundException($"Author with id {input.AuthorId} does not exist.");

            var entity = input.ToEntity();
            _db.Books.Add(entity);
            await _db.SaveChangesAsync(ct);

            var authorName = await _db.Authors
                .Where(a => a.Id == entity.AuthorId)
                .Select(a => a.Name)
                .SingleAsync(ct);

            return new BookDto(entity.Id, entity.Title, entity.Year,
                new AuthorSummaryDto(entity.AuthorId, authorName));
        }

        public async Task<bool> UpdateAsync(int id, BookUpdateDto input, CancellationToken ct = default)
        {
            var entity = await _db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);
            if (entity is null) return false;

            var authorExists = await _db.Authors.AnyAsync(a => a.Id == input.AuthorId, ct);
            if (!authorExists) return false;

            input.UpdateEntity(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.Books.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;

            _db.Books.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
