using System.Collections.ObjectModel;

namespace RaidTeam.Models
{
    public class Group
    {
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public ObservableCollection<GroupSlot> Slots { get; set; } = new();
    }
}