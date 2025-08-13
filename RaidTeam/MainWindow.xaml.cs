using Microsoft.UI.Xaml;
using RaidTeam.Services;
using RaidTeam.ViewModels;

namespace RaidTeam
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly DialogService _dialogService;

        public MainWindow()
        {
            InitializeComponent();

            _dialogService = new DialogService();
            _viewModel = new MainViewModel();

            // Suscripción al evento del ViewModel para abrir el modal
            _viewModel.AddPlayerRequested += async () =>
            {
                return await _dialogService.ShowAddPlayerDialogAsync(this.Content.XamlRoot);
            };

            RootGrid.DataContext = _viewModel;
        }
    }
}