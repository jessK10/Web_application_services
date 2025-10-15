namespace project_1.Dtos;

public record AuthorDto(int Id, string Name, string? Email);

// for create/update – no Id from client
public record AuthorCreateDto(string Name, string? Email);
public record AuthorUpdateDto(string Name, string? Email);
