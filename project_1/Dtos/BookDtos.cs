namespace project_1.Dtos;

// What we return to clients
public record BookDto(int Id, string Title, int Year, AuthorSummaryDto Author);

// Small embedded author shape for books list/details
public record AuthorSummaryDto(int Id, string Name);

// What we accept from clients
public record BookCreateDto(string Title, int Year, int AuthorId);
public record BookUpdateDto(string Title, int Year, int AuthorId);
