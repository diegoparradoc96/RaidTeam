using Microsoft.EntityFrameworkCore;
using RaidTeam.Models;

namespace RaidTeam.Data
{
    public class RaidTeamDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<RaidTeamModel> RaidTeams { get; set; }
        public DbSet<RaidGroup> RaidGroups { get; set; }
        public DbSet<RaidSlot> RaidSlots { get; set; }

        public RaidTeamDbContext(DbContextOptions<RaidTeamDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RaidTeamModel>()
                .HasMany(rt => rt.Groups)
                .WithOne(rg => rg.RaidTeam)
                .HasForeignKey(rg => rg.RaidTeamModelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RaidGroup>()
                .HasMany(rg => rg.Slots)
                .WithOne(rs => rs.RaidGroup)
                .HasForeignKey(rs => rs.RaidGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RaidSlot>()
                .HasOne(rs => rs.Player)
                .WithMany()
                .HasForeignKey(rs => rs.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}