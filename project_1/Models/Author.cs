using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace project_1.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        [EmailAddress, MaxLength(200)]
        public string? Email { get; set; }

        // Avoid JSON cycles: authors -> books -> author -> books...
        [JsonIgnore]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
