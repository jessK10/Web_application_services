using System.ComponentModel.DataAnnotations;

namespace project_1.Dtos
{
    public record PublisherDto(int Id, string Name);
    public record PublisherCreateDto(string Name);
    public record PublisherUpdateDto(string Name);
}

