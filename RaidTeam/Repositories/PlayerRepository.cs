using Microsoft.EntityFrameworkCore;
using RaidTeam.Data;
using RaidTeam.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaidTeam.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly RaidTeamDbContext _context;

        public PlayerRepository(RaidTeamDbContext context)
        {
            _context = context;
        }

        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task AddAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Player player)
        {
            _context.Entry(player).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
        }
    }
}