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
        Task<List<RaidTeamModel>> GetAllAsync();
        Task SaveRaidTeamAsync(RaidTeamModel raidTeam);
        Task<RaidTeamModel> CreateNewRaidTeamAsync(string name);
        Task SetCurrentRaidTeamAsync(int raidTeamId);
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
                .FirstOrDefaultAsync(rt => rt.IsCurrent);
        }

        public async Task<List<RaidTeamModel>> GetAllAsync()
        {
            return await _context.RaidTeams
                .Include(rt => rt.Groups)
                    .ThenInclude(g => g.Slots)
                        .ThenInclude(s => s.Player)
                .ToListAsync();
        }

        public async Task<RaidTeamModel> CreateNewRaidTeamAsync(string name)
        {
            var raidTeam = new RaidTeamModel { Name = name };
                
            for (int i = 1; i <= 6; i++)
            {
                var group = new RaidGroup 
                { 
                    Name = i < 6 ? $"Group {i}" : "Bench",
                    Position = i - 1
                };

                for (int j = 0; j < 5; j++)
                {
                    group.Slots.Add(new RaidSlot { Position = j });
                }

                raidTeam.Groups.Add(group);
            }

            _context.RaidTeams.Add(raidTeam);
            await _context.SaveChangesAsync();
            return raidTeam;
        }

        public async Task SetCurrentRaidTeamAsync(int raidTeamId)
        {
            // Primero, quitar la marca de actual de todas las raids
            var currentRaids = await _context.RaidTeams.Where(rt => rt.IsCurrent).ToListAsync();
            foreach (var raid in currentRaids)
            {
                raid.IsCurrent = false;
            }

            // Marcar la nueva raid como actual
            var newCurrentRaid = await _context.RaidTeams.FindAsync(raidTeamId);
            if (newCurrentRaid != null)
            {
                newCurrentRaid.IsCurrent = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveRaidTeamAsync(RaidTeamModel raidTeam)
        {
            var existing = await _context.RaidTeams
                .Include(rt => rt.Groups)
                    .ThenInclude(g => g.Slots)
                .FirstOrDefaultAsync(rt => rt.Id == raidTeam.Id);

            if (existing == null)
            {
                // Nueva raidteam, agregar todo
                _context.RaidTeams.Add(raidTeam);
            }
            else
            {
                // Actualizar propiedades simples
                existing.Name = raidTeam.Name;
                existing.IsCurrent = raidTeam.IsCurrent;

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