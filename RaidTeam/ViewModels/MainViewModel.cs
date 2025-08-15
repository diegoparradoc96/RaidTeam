using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaidTeam.Models;
using RaidTeam.Repositories;
using System;
using System.Linq;

namespace RaidTeam.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IPlayerRepository _playerRepository;

        public event Func<Task<Player?>>? AddPlayerRequested;
        public event Func<Player, Task<bool>>? DeletePlayerRequested;

        private ObservableCollection<Player> _players = [];
        public ObservableCollection<Player> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
        }

        private ObservableCollection<GroupSlot> _groupSlots = new(
            Enumerable.Range(0, 5).Select(i => new GroupSlot { Position = i })
        );
        public ObservableCollection<GroupSlot> GroupSlots
        {
            get => _groupSlots;
            set => SetProperty(ref _groupSlots, value);
        }

        public MainViewModel(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
            LoadPlayersAsync();
        }

        private async void LoadPlayersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            Players = new ObservableCollection<Player>(players);
        }

        [RelayCommand]
        public async Task AddPlayerAsync()
        {
            if (AddPlayerRequested != null)
            {
                var player = await AddPlayerRequested.Invoke();
                if (player != null)
                {
                    await _playerRepository.AddAsync(player);
                    Players.Add(player);
                }
            }
        }

        [RelayCommand]
        public async Task DeletePlayerAsync(Player player)
        {
            if (DeletePlayerRequested != null)
            {
                var confirm = await DeletePlayerRequested.Invoke(player);
                if (confirm)
                {
                    await _playerRepository.DeleteAsync(player.Id);
                    Players.Remove(player);
                }
            }
        }

        public void AssignPlayerToSlot(GroupSlot slot, Player player)
        {
            slot.Player = player;
            OnPropertyChanged(nameof(GroupSlots));
        }
    }
}