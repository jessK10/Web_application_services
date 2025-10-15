using System.ComponentModel.DataAnnotations;

namespace project_1.Dtos
{
    public record MemberDto(int Id, string Name, string? Email);
    public record MemberCreateDto(string Name, string? Email);
    public record MemberUpdateDto(string Name, string? Email);
}
