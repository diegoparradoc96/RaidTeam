using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;

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

            // Crear imagen para warriors
            var imgWarArms = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Warrior_Arms.png")),
                Width = 25,
                Height = 25
            };
            var imgWarProt = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Warrior_Prot.png")),
                Width = 25,
                Height = 25
            };
            var imgWarFury = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Warrior_Fury.png")),
                Width = 25,
                Height = 25
            };
            // Crear imagen para Paladines
            var imgPalaRetri = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Paladin_Retri.png")),
                Width = 25,
                Height = 25
            };
            var imgPalaProt = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Paladin_Prot.png")),
                Width = 25,
                Height = 25
            };
            var imgPalaHoly = new Image
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/Paladin_Holy.png")),
                Width = 25,
                Height = 25
            };

            // Crear botones para roles de Warrior
            var btnWarArms = new Button
            {
                Content = imgWarArms,
                Margin = new Thickness(1),
                Padding = new Thickness(1, 1, 1, 1)
            };
            var btnWarProt = new Button
            {
                Content = imgWarProt,
                Margin = new Thickness(1),
                Padding = new Thickness(1, 1, 1, 1)
            };
            var btnWarFury = new Button
            {
                Content = imgWarFury,
                Margin = new Thickness(1),
                Padding = new Thickness(1, 1, 1, 1)
            };
            //crear botones para roles de Paladin
            var btnPalaRetri = new Button
            {
                Content = imgPalaRetri,
                Margin = new Thickness(1),
                Padding = new Thickness(1, 1, 1, 1)
            };
            var btnPalaProt = new Button
            {
                Content = imgPalaProt,
                Margin = new Thickness(1),
                Padding = new Thickness(1, 1, 1, 1)
            };
            var btnPalaHoly = new Button
            {
                Content = imgPalaHoly,
                Margin = new Thickness(1),
                Padding = new Thickness(1, 1, 1, 1)
            };

            void SeleccionarRol(Button btn, string rol)
            {
                // Quitar selección a todos
                btnWarArms.ClearValue(Button.BackgroundProperty);
                btnWarProt.ClearValue(Button.BackgroundProperty);
                btnWarFury.ClearValue(Button.BackgroundProperty);
                btnPalaRetri.ClearValue(Button.BackgroundProperty);
                btnPalaProt.ClearValue(Button.BackgroundProperty);
                btnPalaHoly.ClearValue(Button.BackgroundProperty);

                // Seleccionar este
                btn.Background = new SolidColorBrush(Microsoft.UI.Colors.LightBlue);
                rolSeleccionado = rol;
            }

            btnWarArms.Click += (s, e) => SeleccionarRol(btnWarArms, "Warrior Arms");
            btnWarProt.Click += (s, e) => SeleccionarRol(btnWarProt, "Warrior Prot");
            btnWarFury.Click += (s, e) => SeleccionarRol(btnWarFury, "Warrior Fury");

            btnPalaRetri.Click += (s, e) => SeleccionarRol(btnPalaRetri, "Paladin Retribution");
            btnPalaProt.Click += (s, e) => SeleccionarRol(btnPalaProt, "Paladin Protection");
            btnPalaHoly.Click += (s, e) => SeleccionarRol(btnPalaHoly, "Paladin Holy");

            var panelWarRoles = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var panelPalaRoles = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            panelWarRoles.Children.Add(btnWarArms);
            panelWarRoles.Children.Add(btnWarProt);
            panelWarRoles.Children.Add(btnWarFury);

            panelPalaRoles.Children.Add(btnPalaRetri);
            panelPalaRoles.Children.Add(btnPalaProt);
            panelPalaRoles.Children.Add(btnPalaHoly);

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(inputBox);
            stackPanel.Children.Add(panelWarRoles);
            stackPanel.Children.Add(panelPalaRoles);

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