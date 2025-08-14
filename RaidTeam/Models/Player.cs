using System.ComponentModel.DataAnnotations;

namespace RaidTeam.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Role { get; set; } = "";

        public string IconPath { get; set; } = "";
    }
}