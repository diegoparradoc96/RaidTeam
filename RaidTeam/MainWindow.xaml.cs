using Microsoft.UI.Xaml;
using RaidTeam.Services;
using RaidTeam.ViewModels;

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

            RootGrid.DataContext = _viewModel;
        }
    }
}