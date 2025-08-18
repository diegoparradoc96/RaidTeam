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
        private ObservableCollection<Player> _allPlayers = [];

        public event Func<Task<Player?>>? AddPlayerRequested;
        public event Func<Player, Task<bool>>? DeletePlayerRequested;

        [ObservableProperty]
        private string? _selectedRole;

        [ObservableProperty]
        private string _currentFilterIcon = ""; // Ícono actual del botón de filtro

        [ObservableProperty]
        private string _searchText = "";

        private ObservableCollection<Player> _players = [];
        public ObservableCollection<Player> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
        }

        private ObservableCollection<Group> _groups = new(
            Enumerable.Range(0, 6).Select(i => new Group
            {
                Name = i < 5 ? $"Group {i + 1}" : "Bench",
                Position = i,
                Slots = new ObservableCollection<GroupSlot>(
                    Enumerable.Range(0, 5).Select(j => new GroupSlot { Position = j })
                )
            })
        );
        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public MainViewModel(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
            LoadPlayersAsync();
        }

        private async void LoadPlayersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            _allPlayers = new ObservableCollection<Player>(players);
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
                    _allPlayers.Add(player);
                    FilterPlayers();
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
                    _allPlayers.Remove(player);
                    FilterPlayers();
                }
            }
        }

        [RelayCommand]
        public void FilterByRole(string role)
        {
            SelectedRole = role;
            FilterPlayers();
        }

        [RelayCommand]
        public void FilterByRoleWithIcon(string parameter)
        {
            if (!string.IsNullOrEmpty(parameter))
            {
                var parts = parameter.Split(',');
                if (parts.Length == 2)
                {
                    SelectedRole = parts[0];
                    CurrentFilterIcon = parts[1];
                    FilterPlayers();
                }
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterPlayers();
        }

        private void FilterPlayers()
        {
            var filteredPlayers = _allPlayers.AsEnumerable();

            // Filtrar por rol si hay uno seleccionado
            if (!string.IsNullOrEmpty(SelectedRole))
            {
                filteredPlayers = filteredPlayers.Where(p => p.Role.StartsWith(SelectedRole));
            }

            // Filtrar por texto de búsqueda si hay uno
            if (!string.IsNullOrEmpty(SearchText))
            {
                filteredPlayers = filteredPlayers.Where(p => 
                    p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            Players = new ObservableCollection<Player>(filteredPlayers);
        }

        [RelayCommand]
        public void ClearFilter()
        {
            SelectedRole = null;
            CurrentFilterIcon = ""; // Resetear el ícono cuando se limpia el filtro
            FilterPlayers();
        }

        public void AssignPlayerToSlot(GroupSlot slot, Player player)
        {
            slot.Player = player;
            OnPropertyChanged(nameof(Groups));
        }

        [RelayCommand]
        public void RemovePlayerFromSlot(GroupSlot slot)
        {
            if (slot != null)
            {
                slot.Player = null;
            }
        }
    }
}