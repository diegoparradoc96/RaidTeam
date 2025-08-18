using System.Collections.Generic;
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
        private readonly IRaidTeamRepository _raidTeamRepository;
        private ObservableCollection<Player> _allPlayers = [];
        private RaidTeamModel? _currentRaidTeam;
        private Dictionary<(int groupPosition, int slotPosition), int> _slotIds = new();

        [ObservableProperty]
        private string _raidTeamName = "";

        public event Func<Task<Player?>>? AddPlayerRequested;
        public event Func<Player, Task<bool>>? DeletePlayerRequested;
        public event Func<Task<string?>>? EditRaidNameRequested;

        [ObservableProperty]
        private string? _selectedRole;

        [ObservableProperty]
        private string? _currentFilterIcon = null;

        [ObservableProperty]
        private string _searchText = "";

        private ObservableCollection<Player> _players = [];
        public ObservableCollection<Player> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
        }

        private ObservableCollection<Group> _groups = [];
        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public MainViewModel(IPlayerRepository playerRepository, IRaidTeamRepository raidTeamRepository)
        {
            _playerRepository = playerRepository;
            _raidTeamRepository = raidTeamRepository;
            LoadPlayersAsync();
            LoadRaidTeamAsync();
        }

        private async void LoadPlayersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            _allPlayers = new ObservableCollection<Player>(players.OrderBy(p => GetClassOrder(p.Role)));
            Players = new ObservableCollection<Player>(_allPlayers);
        }

        private async void LoadRaidTeamAsync()
        {
            _currentRaidTeam = await _raidTeamRepository.GetCurrentAsync();
            if (_currentRaidTeam == null)
            {
                _currentRaidTeam = CreateDefaultRaidTeam();
                await _raidTeamRepository.SaveRaidTeamAsync(_currentRaidTeam);
            }

            RaidTeamName = _currentRaidTeam.Name;

            // Store slot IDs for later use
            _slotIds.Clear();
            foreach (var group in _currentRaidTeam.Groups)
            {
                foreach (var slot in group.Slots)
                {
                    _slotIds[(group.Position, slot.Position)] = slot.Id;
                }
            }

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
                    Name = i < 6 ? $"Group {i}" : "Bench",
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

        private int GetClassOrder(string? role)
        {
            if (string.IsNullOrEmpty(role))
                return 999; // Poner al final los roles nulos o vacíos

            // Extraer la clase base y especialización
            var parts = role.Split(' ');
            var baseClass = parts[0];
            var spec = parts.Length > 1 ? parts[1] : "";
            
            // Orden principal por clase
            var classOrder = baseClass switch
            {
                "Warrior" => 1,
                "Paladin" => 2,
                "Hunter" => 3,
                "Rogue" => 4,
                "Priest" => 5,
                "Shaman" => 6,
                "Mage" => 7,
                "Warlock" => 8,
                "Druid" => 9,
                _ => 10
            };

            // Orden secundario por especialización
            var specOrder = baseClass switch
            {
                "Warrior" => GetWarriorSpecOrder(spec),
                "Paladin" => GetPaladinSpecOrder(spec),
                "Hunter" => GetHunterSpecOrder(spec),
                "Rogue" => GetRogueSpecOrder(spec),
                "Priest" => GetPriestSpecOrder(spec),
                "Shaman" => GetShamanSpecOrder(spec),
                "Mage" => GetMageSpecOrder(spec),
                "Warlock" => GetWarlockSpecOrder(spec),
                "Druid" => GetDruidSpecOrder(spec),
                _ => 0
            };

            // Combinar orden de clase y especialización
            return (classOrder * 100) + specOrder;
        }

        private static int GetWarriorSpecOrder(string spec) => spec switch
        {
            "Arms" => 1,
            "Fury" => 2,
            "Protection" => 3,
            _ => 0
        };

        private static int GetPaladinSpecOrder(string spec) => spec switch
        {
            "Holy" => 1,
            "Protection" => 2,
            "Retribution" => 3,
            _ => 0
        };

        private static int GetHunterSpecOrder(string spec) => spec switch
        {
            "Beast" => 1, // Beast Mastery
            "Marksmanship" => 2,
            "Survival" => 3,
            _ => 0
        };

        private static int GetRogueSpecOrder(string spec) => spec switch
        {
            "Assassination" => 1,
            "Combat" => 2,
            "Subtlety" => 3,
            _ => 0
        };

        private static int GetPriestSpecOrder(string spec) => spec switch
        {
            "Disciplinary" => 1,
            "Holy" => 2,
            "Shadow" => 3,
            _ => 0
        };

        private static int GetShamanSpecOrder(string spec) => spec switch
        {
            "Elemental" => 1,
            "Enhancement" => 2,
            "Restoration" => 3,
            _ => 0
        };

        private static int GetMageSpecOrder(string spec) => spec switch
        {
            "Arcane" => 1,
            "Fire" => 2,
            "Frost" => 3,
            _ => 0
        };

        private static int GetWarlockSpecOrder(string spec) => spec switch
        {
            "Affliction" => 1,
            "Demonology" => 2,
            "Destruction" => 3,
            _ => 0
        };

        private static int GetDruidSpecOrder(string spec) => spec switch
        {
            "Balance" => 1,
            "Feral" => 2,
            "Restoration" => 3,
            _ => 0
        };

        private void FilterPlayers()
        {
            IEnumerable<Player> filteredPlayers = _allPlayers;

            // Aplicar filtros si existen
            if (!string.IsNullOrEmpty(SelectedRole))
            {
                filteredPlayers = filteredPlayers.Where(p => p.Role?.StartsWith(SelectedRole) == true);
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                filteredPlayers = filteredPlayers.Where(p => 
                    p.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
            }

            // Ordenar siempre por clase y especialización
            filteredPlayers = filteredPlayers.OrderBy(p => GetClassOrder(p.Role));

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

        [RelayCommand]
        private async Task EditRaidTeamNameAsync()
        {
            if (EditRaidNameRequested != null && _currentRaidTeam != null)
            {
                var newName = await EditRaidNameRequested.Invoke();
                if (!string.IsNullOrEmpty(newName))
                {
                    RaidTeamName = newName;
                    _currentRaidTeam.Name = newName;
                    await _raidTeamRepository.SaveRaidTeamAsync(_currentRaidTeam);
                }
            }
        }
    }
}