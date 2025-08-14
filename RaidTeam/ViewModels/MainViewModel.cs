using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaidTeam.Models;
using RaidTeam.Repositories;
using System;

namespace RaidTeam.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IPlayerRepository _playerRepository;
        public event Func<Task<Player?>>? AddPlayerRequested;

        private ObservableCollection<Player> _players = [];

        public ObservableCollection<Player> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
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
    }
}