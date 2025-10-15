// Mapping/DtoMappings.cs
using project_1.Dtos;
using project_1.Models;

namespace project_1.Mapping
{
    public static class DtoMappings
    {
        // -------- Author --------
        public static AuthorDto ToDto(this Author a)
            => new(a.Id, a.Name, a.Email);

        public static Author ToEntity(this AuthorCreateDto d)
            => new() { Name = d.Name, Email = d.Email };

        public static void UpdateEntity(this AuthorUpdateDto d, Author e)
        {
            e.Name = d.Name;
            e.Email = d.Email;
        }

        // -------- Book --------
        public static BookDto ToDto(this Book b)
            => new(
                b.Id,
                b.Title,
                b.Year,
                new AuthorSummaryDto(
                    b.AuthorId,
                    b.Author?.Name ?? "Unknown Author"
                )
            );

        public static Book ToEntity(this BookCreateDto d)
            => new() { Title = d.Title, Year = d.Year, AuthorId = d.AuthorId };

        public static void UpdateEntity(this BookUpdateDto d, Book e)
        {
            e.Title = d.Title;
            e.Year = d.Year;
            e.AuthorId = d.AuthorId;
        }

        // -------- Publisher --------
        public static PublisherDto ToDto(this Publisher p)
            => new(p.Id, p.Name);

        public static Publisher ToEntity(this PublisherCreateDto d)
            => new() { Name = d.Name };

        public static void UpdateEntity(this PublisherUpdateDto d, Publisher e)
            => e.Name = d.Name;

        // -------- Genre --------
        public static GenreDto ToDto(this Genre g)
            => new(g.Id, g.Name);

        public static Genre ToEntity(this GenreCreateDto d)
            => new() { Name = d.Name };

        public static void UpdateEntity(this GenreUpdateDto d, Genre e)
            => e.Name = d.Name;

        // -------- Member --------
        public static MemberDto ToDto(this Member m)
            => new(m.Id, m.Name, m.Email);

        public static Member ToEntity(this MemberCreateDto d)
            => new() { Name = d.Name, Email = d.Email };

        public static void UpdateEntity(this MemberUpdateDto d, Member e)
        {
            e.Name = d.Name;
            e.Email = d.Email;
        }
    }
}
