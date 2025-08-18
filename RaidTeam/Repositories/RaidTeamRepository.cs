using System.Collections.Generic;
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
                // Nueva raidteam, agregar todo
                _context.RaidTeams.Add(raidTeam);
            }
            else
            {
                // Actualizar propiedades simples
                existing.Name = raidTeam.Name;

                // Actualizar grupos
                foreach (var groupModel in raidTeam.Groups)
                {
                    var existingGroup = existing.Groups.FirstOrDefault(g => g.Position == groupModel.Position);
                    if (existingGroup == null)
                    {
                        // Nuevo grupo
                        var newGroup = new RaidGroup
                        {
                            Name = groupModel.Name,
                            Position = groupModel.Position,
                            RaidTeamModelId = existing.Id
                        };

                        // Agregar slots del grupo
                        foreach (var slotModel in groupModel.Slots)
                        {
                            newGroup.Slots.Add(new RaidSlot
                            {
                                Position = slotModel.Position,
                                PlayerId = slotModel.PlayerId
                            });
                        }
                        existing.Groups.Add(newGroup);
                    }
                    else
                    {
                        // Actualizar nombre
                        existingGroup.Name = groupModel.Name;
                        
                        // Actualizar slots
                        foreach (var slotModel in groupModel.Slots)
                        {
                            var existingSlot = existingGroup.Slots.FirstOrDefault(s => s.Position == slotModel.Position);
                            if (existingSlot == null)
                            {
                                existingGroup.Slots.Add(new RaidSlot
                                {
                                    Position = slotModel.Position,
                                    PlayerId = slotModel.PlayerId
                                });
                            }
                            else
                            {
                                existingSlot.PlayerId = slotModel.PlayerId;
                            }
                        }
                    }
                }
            }
            
            await _context.SaveChangesAsync();
        }
    }
}