using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaidTeam.Models;
using RaidTeam.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RaidTeam.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRaidTeamRepository _raidTeamRepository;
        private readonly ObservableCollection<Player> _allPlayers;
        private RaidTeamModel? _currentRaidTeam;
        private Dictionary<(int groupPosition, int slotPosition), int> _slotIds = new();

        [ObservableProperty]
        private ObservableCollection<Group>? _groups;

        [ObservableProperty]
        private ObservableCollection<Player>? _players;

        [ObservableProperty]
        private string? _searchText;

        [ObservableProperty]
        private string? _selectedRole;

        [ObservableProperty]
        private string? _currentFilterIcon;

        public event Func<Task<Player?>>? AddPlayerRequested;
        public event Func<Player, Task<bool>>? DeletePlayerRequested;

        public MainViewModel(IPlayerRepository playerRepository, IRaidTeamRepository raidTeamRepository)
        {
            _playerRepository = playerRepository;
            _raidTeamRepository = raidTeamRepository;
            _allPlayers = new ObservableCollection<Player>();
            Players = new ObservableCollection<Player>();
            Groups = new ObservableCollection<Group>();

            InitializeGroupsAsync().ConfigureAwait(false);
        }

        private async Task InitializeGroupsAsync()
        {
            // Initialize players
            var dbPlayers = await _playerRepository.GetAllAsync();
            foreach (var player in dbPlayers)
            {
                _allPlayers.Add(player);
            }
            FilterPlayers();

            // Initialize groups from database or create new
            _currentRaidTeam = await _raidTeamRepository.GetCurrentAsync();
            if (_currentRaidTeam == null)
            {
                // Create default groups if no raid team exists
                _currentRaidTeam = CreateDefaultRaidTeam();
                await _raidTeamRepository.SaveRaidTeamAsync(_currentRaidTeam);
            }

            // Store slot IDs for later use
            foreach (var group in _currentRaidTeam.Groups)
            {
                foreach (var slot in group.Slots)
                {
                    _slotIds[(group.Position, slot.Position)] = slot.Id;
                }
            }

            // Convert RaidTeam to UI Groups
            Groups = new ObservableCollection<Group>(
                _currentRaidTeam.Groups.OrderBy(g => g.Position).Select(g => new Group
                {
                    Name = g.Name,
                    Position = g.Position,
                    Slots = new ObservableCollection<GroupSlot>(
                        g.Slots.OrderBy(s => s.Position).Select(s => new GroupSlot
                        {
                            Position = s.Position,
                            Player = s.Player
                        }))
                }));
        }

        private RaidTeamModel CreateDefaultRaidTeam()
        {
            var raidTeam = new RaidTeamModel { Name = "Default" };
            
            for (int i = 1; i <= 6; i++)
            {
                var group = new RaidGroup 
                { 
                    Name = $"Group {i}",
                    Position = i - 1
                };

                for (int j = 0; j < 5; j++)
                {
                    group.Slots.Add(new RaidSlot { Position = j });
                }

                raidTeam.Groups.Add(group);
            }

            return raidTeam;
        }

        private async Task SaveRaidTeamStateAsync()
        {
            if (Groups == null || _currentRaidTeam == null) return;

            // Actualizar el modelo de datos
            foreach (var group in Groups)
            {
                var dbGroup = _currentRaidTeam.Groups.FirstOrDefault(g => g.Position == group.Position);
                if (dbGroup != null)
                {
                    foreach (var slot in group.Slots)
                    {
                        var slotId = _slotIds[(group.Position, slot.Position)];
                        var dbSlot = dbGroup.Slots.FirstOrDefault(s => s.Id == slotId);
                        if (dbSlot != null)
                        {
                            dbSlot.PlayerId = slot.Player?.Id;
                        }
                    }
                }
            }

            // Guardar cambios en la base de datos
            await _raidTeamRepository.SaveRaidTeamAsync(_currentRaidTeam);
        }

        [RelayCommand]
        private async Task AddPlayerAsync()
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
                    
                    // Remove player from any slots
                    if (Groups != null)
                    {
                        foreach (var group in Groups)
                        {
                            foreach (var slot in group.Slots)
                            {
                                if (slot.Player?.Id == player.Id)
                                {
                                    slot.Player = null;
                                }
                            }
                        }
                    }
                    
                    await SaveRaidTeamStateAsync();
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

            if (!string.IsNullOrEmpty(SelectedRole))
            {
                filteredPlayers = filteredPlayers.Where(p => p.Role.StartsWith(SelectedRole));
            }

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
            CurrentFilterIcon = null;
            FilterPlayers();
        }

        public async Task AssignPlayerToSlot(GroupSlot slot, Player player)
        {
            slot.Player = player;
            OnPropertyChanged(nameof(Groups));
            await SaveRaidTeamStateAsync();
        }

        [RelayCommand]
        public async Task RemovePlayerFromSlot(GroupSlot slot)
        {
            if (slot != null)
            {
                slot.Player = null;
                await SaveRaidTeamStateAsync();
            }
        }
    }
}