using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using RaidTeam.Models;
using RaidTeam.Services;
using RaidTeam.ViewModels;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace RaidTeam
{
    public sealed partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly IDialogService _dialogService;
        private GroupSlot? _dragSourceSlot;

        public MainWindow(MainViewModel viewModel, IDialogService dialogService)
        {
            InitializeComponent();

            _dialogService = dialogService;
            _viewModel = viewModel;

            // Suscripción al evento del ViewModel para abrir el modal
            _viewModel.AddPlayerRequested += async () =>
                await _dialogService.ShowAddPlayerDialogAsync(Content.XamlRoot);

            _viewModel.DeletePlayerRequested += async (player) =>
                await _dialogService.ShowDeletePlayerConfirmationAsync(Content.XamlRoot, player);

            RootGrid.DataContext = _viewModel;
        }

        // Drag & Drop para jugadores desde la lista
        private void PlayersListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items.FirstOrDefault() is Player player)
            {
                e.Data.Properties["Player"] = player;
                e.Data.RequestedOperation = DataPackageOperation.Copy;
            }
        }

        // Drag & Drop para jugadores dentro de slots
        private void SlotPlayer_DragStarting(UIElement sender, DragStartingEventArgs e)
        {
            if (sender.GetType().GetProperty("DataContext")?.GetValue(sender) is GroupSlot sourceSlot && 
                sourceSlot.Player != null)
            {
                _dragSourceSlot = sourceSlot;
                e.Data.Properties["SlotPlayer"] = sourceSlot.Player;
                e.Data.RequestedOperation = DataPackageOperation.Move;
            }
        }

        private void PlayerSlot_DragOver(object sender, DragEventArgs e)
        {
            // Aceptar drag desde la lista de jugadores
            if (e.DataView.Properties.ContainsKey("Player"))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.Caption = "Asignar jugador a slot";
            }
            // Aceptar drag desde otro slot (de cualquier grupo)
            else if (e.DataView.Properties.ContainsKey("SlotPlayer") && 
                     sender is FrameworkElement element && 
                     element.DataContext is GroupSlot targetSlot)
            {
                if (targetSlot != _dragSourceSlot)
                {
                    e.AcceptedOperation = DataPackageOperation.Move;
                    e.DragUIOverride.Caption = "Mover jugador a este slot";
                }
            }
        }

        private void PlayerSlot_Drop(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is GroupSlot targetSlot)
            {
                // Drop desde la lista de jugadores
                if (e.DataView.Properties.TryGetValue("Player", out var playerObj) && 
                    playerObj is Player player)
                {
                    _viewModel.AssignPlayerToSlot(targetSlot, player);
                }
                // Drop desde otro slot (de cualquier grupo)
                else if (e.DataView.Properties.TryGetValue("SlotPlayer", out var slotPlayerObj) && 
                         slotPlayerObj is Player slotPlayer &&
                         _dragSourceSlot != null && targetSlot != _dragSourceSlot)
                {
                    // Intercambiar jugadores entre slots
                    var tempPlayer = targetSlot.Player;
                    targetSlot.Player = slotPlayer;
                    _dragSourceSlot.Player = tempPlayer;
                }
            }
        }

        private void FilterButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (_viewModel.ClearFilterCommand.CanExecute(null))
            {
                _viewModel.ClearFilterCommand.Execute(null);
            }
        }
    }
}