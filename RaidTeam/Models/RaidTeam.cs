using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RaidTeam.Models
{
    public class RaidTeamModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "Default";
        public List<RaidGroup> Groups { get; set; } = new();
    }

    public class RaidGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public int RaidTeamId { get; set; }
        public RaidTeamModel RaidTeam { get; set; } = null!;
        public List<RaidSlot> Slots { get; set; } = new();
    }

    public class RaidSlot
    {
        [Key]
        public int Id { get; set; }
        public int Position { get; set; }
        public int RaidGroupId { get; set; }
        public RaidGroup RaidGroup { get; set; } = null!;
        public int? PlayerId { get; set; }
        public Player? Player { get; set; }
    }
}