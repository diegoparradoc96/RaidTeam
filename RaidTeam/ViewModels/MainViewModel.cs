using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaidTeam.Models;
using System;

namespace RaidTeam.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public event Func<Task<Player>>? AddPlayerRequested;

        private ObservableCollection<Player> _players = [];

        public ObservableCollection<Player> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
        }

        [RelayCommand]
        public async Task AddPlayerAsync()
        {
            if (AddPlayerRequested != null)
            {
                var player = await AddPlayerRequested.Invoke();
                if (player != null)
                {
                    Players.Add(player);
                }
            }
        }
    }
}