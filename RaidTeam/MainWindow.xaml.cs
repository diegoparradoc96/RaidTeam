using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using RaidTeam.Models;
using RaidTeam.Services;
using RaidTeam.ViewModels;
using System.Linq;

namespace RaidTeam
{
    public sealed partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly IDialogService _dialogService;

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

        // Drag & Drop para jugadores
        private void PlayersListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            if (e.Items.FirstOrDefault() is Player player)
            {
                e.Data.Properties["Player"] = player;
                e.Data.RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            }
        }

        private void PlayerSlot_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey("Player"))
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
                e.DragUIOverride.Caption = "Asignar jugador a slot";
            }
        }

        private void PlayerSlot_Drop(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is GroupSlot slot &&
                e.DataView.Properties.TryGetValue("Player", out var playerObj) && playerObj is Player player)
            {
                _viewModel.AssignPlayerToSlot(slot, player);
            }
        }
    }
}