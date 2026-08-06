using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class NoteDbContext(DbContextOptions<NoteDbContext> options) : DbContext(options)
    {
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<User> Users => Set<User>();
    }
}
