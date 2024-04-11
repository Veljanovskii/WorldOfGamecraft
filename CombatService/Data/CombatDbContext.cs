using CombatService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CombatService.Data;

public class CombatDbContext : DbContext
{
    public CombatDbContext(DbContextOptions<CombatDbContext> options) : base(options)
    {
    }

    public DbSet<Duel> Duels { get; set; }
    public DbSet<DuelAction> DuelActions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Duel>()
            .HasMany(d => d.DuelActions)
            .WithOne()
            .HasForeignKey(da => da.DuelId);
    }
}
