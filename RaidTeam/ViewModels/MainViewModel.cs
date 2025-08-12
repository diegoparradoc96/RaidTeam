using RaidTeam.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace RaidTeam.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<string> jugadores = new();

        public MainViewModel(DialogService dialogService)
        {
            _dialogService = dialogService;
        }

        [RelayCommand]
        public async Task AddPlayerAsync()
        {            
            var nombreJugador = await _dialogService.ShowAddPlayerDialogAsync();
            if (!string.IsNullOrWhiteSpace(nombreJugador))
            {
                Jugadores.Add(nombreJugador);
            }
        }
    }
}