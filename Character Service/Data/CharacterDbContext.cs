using CharacterService.Models;
using Microsoft.EntityFrameworkCore;

namespace CharacterService.Data;

public class CharacterDbContext : DbContext
{
    public CharacterDbContext(DbContextOptions<CharacterDbContext> options) : base(options)
    {
    }

    public DbSet<Character> Characters { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>()
            .HasMany(c => c.Characters)
            .WithOne(ch => ch.Class)
            .HasForeignKey(ch => ch.ClassId);

        modelBuilder.Entity<Character>()
            .HasMany(ch => ch.Items)
            .WithMany(i => i.Characters)
            .UsingEntity(j => j.ToTable("CharacterItems"));

        modelBuilder.Entity<Class>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Character>()
            .HasIndex(ch => ch.Name)
            .IsUnique();
    }
}
