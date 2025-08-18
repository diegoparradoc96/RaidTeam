using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RaidTeam.Data;
using RaidTeam.Models;
using System.Linq;

namespace RaidTeam.Repositories
{
    public interface IRaidTeamRepository
    {
        Task<RaidTeamModel?> GetCurrentAsync();
        Task SaveRaidTeamAsync(RaidTeamModel raidTeam);
        Task UpdateSlotAsync(int slotId, int? playerId);
    }

    public class RaidTeamRepository : IRaidTeamRepository
    {
        private readonly RaidTeamDbContext _context;

        public RaidTeamRepository(RaidTeamDbContext context)
        {
            _context = context;
        }

        public async Task<RaidTeamModel?> GetCurrentAsync()
        {
            return await _context.RaidTeams
                .Include(rt => rt.Groups)
                    .ThenInclude(g => g.Slots)
                        .ThenInclude(s => s.Player)
                .FirstOrDefaultAsync();
        }

        public async Task SaveRaidTeamAsync(RaidTeamModel raidTeam)
        {
            var existing = await _context.RaidTeams
                .Include(rt => rt.Groups)
                    .ThenInclude(g => g.Slots)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                _context.RaidTeams.Add(raidTeam);
            }
            else
            {
                // Actualizar propiedades básicas
                _context.Entry(existing).CurrentValues.SetValues(raidTeam);

                // Sincronizar grupos
                foreach (var existingGroup in existing.Groups.ToList())
                {
                    if (!raidTeam.Groups.Any(g => g.Position == existingGroup.Position))
                    {
                        _context.RaidGroups.Remove(existingGroup);
                    }
                }

                foreach (var groupModel in raidTeam.Groups)
                {
                    var existingGroup = existing.Groups.FirstOrDefault(g => g.Position == groupModel.Position);

                    if (existingGroup == null)
                    {
                        // Nuevo grupo
                        existing.Groups.Add(groupModel);
                    }
                    else
                    {
                        // Actualizar grupo existente
                        _context.Entry(existingGroup).CurrentValues.SetValues(groupModel);

                        // Sincronizar slots
                        foreach (var existingSlot in existingGroup.Slots.ToList())
                        {
                            if (!groupModel.Slots.Any(s => s.Position == existingSlot.Position))
                            {
                                _context.RaidSlots.Remove(existingSlot);
                            }
                        }

                        foreach (var slotModel in groupModel.Slots)
                        {
                            var existingSlot = existingGroup.Slots.FirstOrDefault(s => s.Position == slotModel.Position);

                            if (existingSlot == null)
                            {
                                // Nuevo slot
                                existingGroup.Slots.Add(slotModel);
                            }
                            else
                            {
                                // Actualizar slot existente
                                existingSlot.PlayerId = slotModel.PlayerId;
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateSlotAsync(int slotId, int? playerId)
        {
            var slot = await _context.RaidSlots.FindAsync(slotId);
            if (slot != null)
            {
                slot.PlayerId = playerId;
                await _context.SaveChangesAsync();
            }
        }
    }
}