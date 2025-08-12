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
        public MainWindow()
        {
            InitializeComponent();

            // Crear el servicio de diálogos usando el XamlRoot de la ventana
            var dialogService = new DialogService(this.Content.XamlRoot);
            // Crear el ViewModel e inyectar el servicio
            var viewModel = new MainViewModel(dialogService);
            RootGrid.DataContext = viewModel;
            //RootGrid.DataContext = new MainViewModel(viewModel);
        }
    }
}