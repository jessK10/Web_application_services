using System.ComponentModel.DataAnnotations;

namespace project_1.Dtos
{
    public record GenreDto(int Id, string Name);
    public record GenreCreateDto(string Name);
    public record GenreUpdateDto(string Name);
}
