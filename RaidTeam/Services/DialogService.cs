using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;
using Microsoft.UI;

namespace RaidTeam.Services
{
    public class DialogService
    {
        public async Task<string?> ShowAddPlayerDialogAsync(XamlRoot xamlRoot)
        {
            var inputBox = new TextBox
            {
                PlaceholderText = "Nombre del jugador",
                Margin = new Thickness(0, 10, 0, 0)
            };

            string? rolSeleccionado = null;

            var btnRol1 = new Button
            {
                Content = "Tanque",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5)
            };

            var btnRol2 = new Button
            {
                Content = "Sanador",
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5)
            };

            void SeleccionarRol(Button btn, string rol)
            {
                // Quitar selección a todos
                btnRol1.ClearValue(Button.BackgroundProperty);
                btnRol2.ClearValue(Button.BackgroundProperty);

                // Seleccionar este
                btn.Background = new SolidColorBrush(Microsoft.UI.Colors.LightBlue);
                rolSeleccionado = rol;
            }

            btnRol1.Click += (s, e) => SeleccionarRol(btnRol1, "Tanque");
            btnRol2.Click += (s, e) => SeleccionarRol(btnRol2, "Sanador");

            var panelRoles = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            panelRoles.Children.Add(btnRol1);
            panelRoles.Children.Add(btnRol2);

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(inputBox);
            stackPanel.Children.Add(panelRoles);

            var dialog = new ContentDialog
            {
                Title = "Agregar Jugador",
                //Content = inputBox,
                Content = stackPanel,
                PrimaryButtonText = "Guardar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
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