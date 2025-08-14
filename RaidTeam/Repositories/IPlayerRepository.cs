using System.Collections.Generic;
using System.Threading.Tasks;
using RaidTeam.Models;

namespace RaidTeam.Repositories
{
    public interface IPlayerRepository
    {
        Task<List<Player>> GetAllAsync();

        Task<Player?> GetByIdAsync(int id);

        Task AddAsync(Player player);

        Task UpdateAsync(Player player);

        Task DeleteAsync(int id);
    }
}