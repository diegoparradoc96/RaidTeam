using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace RaidTeam.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public event Func<Task<string?>>? AddPlayerRequested;

        private ObservableCollection<string> _players = [];

        public ObservableCollection<string> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
        }

        [RelayCommand]
        public async Task AddPlayerAsync()
        {
            if (AddPlayerRequested != null)
            {
                var playerName = await AddPlayerRequested.Invoke();
                if (!string.IsNullOrWhiteSpace(playerName))
                {
                    Players.Add(playerName);
                }
            }
        }
    }
}