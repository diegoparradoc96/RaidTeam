using Microsoft.EntityFrameworkCore;
using RaidTeam.Models;

namespace RaidTeam.Data
{
    public class RaidTeamDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }

        public RaidTeamDbContext(DbContextOptions<RaidTeamDbContext> options)
            : base(options)
        {
        }
    }
}