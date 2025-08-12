using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace RaidTeam.Services
{
    public class DialogService
    {
        private readonly XamlRoot _xamlRoot;

        public DialogService(XamlRoot xamlRoot)
        {
            _xamlRoot = xamlRoot;
        }

        public async Task<string?> ShowAddPlayerDialogAsync()
        {
            var inputBox = new TextBox
            {
                PlaceholderText = "Nombre del jugador",
                Margin = new Thickness(0, 10, 0, 0)
            };

            var dialog = new ContentDialog
            {
                Title = "Agregar Jugador",
                Content = inputBox,
                PrimaryButtonText = "Guardar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = _xamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                return inputBox.Text.Trim();
            }

            return null;
        }
    }
}