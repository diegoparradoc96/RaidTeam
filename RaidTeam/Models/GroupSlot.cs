using CommunityToolkit.Mvvm.ComponentModel;
using RaidTeam.Models;

namespace RaidTeam.Models
{
    public partial class GroupSlot : ObservableObject
    {
        private int _position;
        [ObservableProperty]
        private Player? _player;

        public int Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }
    }
}
