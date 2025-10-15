using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project_1.Models;

namespace project_1.Data
{
    public class project_1Context : IdentityDbContext<IdentityUser>
    {
        public project_1Context(DbContextOptions<project_1Context> options) : base(options) { }

        // Existing
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();

        // NEW: three simple entities
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Member> Members => Set<Member>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Author
            b.Entity<Author>(e =>
            {
                e.ToTable("Authors");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).UseIdentityColumn();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200);
            });

            // Book
            b.Entity<Book>(e =>
            {
                e.ToTable("Books");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).UseIdentityColumn();
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();

                e.HasOne(x => x.Author)
                 .WithMany(a => a.Books)
                 .HasForeignKey(x => x.AuthorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // NEW: Publisher
            b.Entity<Publisher>(e =>
            {
                e.ToTable("Publishers");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).UseIdentityColumn();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200);
                e.HasIndex(x => x.Name); // optional helpful index
            });

            // NEW: Genre
            b.Entity<Genre>(e =>
            {
                e.ToTable("Genres");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).UseIdentityColumn();
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.HasIndex(x => x.Name).IsUnique(); // genres typically unique
            });

            // NEW: Member
            b.Entity<Member>(e =>
            {
                e.ToTable("Members");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).UseIdentityColumn();
                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200);
                e.HasIndex(x => x.Email); // optional, not unique if you don't want to enforce
            });
        }
    }
}
