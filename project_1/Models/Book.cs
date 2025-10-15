using System.ComponentModel.DataAnnotations;

namespace project_1.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        [Range(0, 2100)]
        public int Year { get; set; }

        // FK -> Author
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
