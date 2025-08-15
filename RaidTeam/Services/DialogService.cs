using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using RaidTeam.Models;
using System;
using System.Threading.Tasks;

namespace RaidTeam.Services
{
    public class DialogService : IDialogService
    {
        private string? rolSeleccionado = null;
        private string? rolIconoSeleccionado = null;
        private Button? botonSeleccionado = null;

        public async Task<Player?> ShowAddPlayerDialogAsync(XamlRoot xamlRoot)
        {
            var inputBox = new TextBox
            {
                PlaceholderText = "Nombre del jugador",
                Margin = new Thickness(0, 10, 0, 0)
            };

            // Contenedor general para todas las clases
            var panelClases = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var panelClases2 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var panelClases3 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // Warrior
            AddClase(panelClases, CreateClassContainer("Warrior", new (string, string)[]
            {
                ("Warrior Arms", "ms-appx:///Assets/Warrior_Arms.png"),
                ("Warrior Fury", "ms-appx:///Assets/Warrior_Fury.png"),
                ("Warrior Protection", "ms-appx:///Assets/Warrior_Prot.png")
            }));

            // Paladin
            AddClase(panelClases, CreateClassContainer("Paladin",
            [
                ("Paladin Holy", "ms-appx:///Assets/Paladin_Holy.png"),
                ("Paladin Protection", "ms-appx:///Assets/Paladin_Prot.png"),
                ("Paladin Retribution", "ms-appx:///Assets/Paladin_Retri.png")
            ]));

            // Hunter
            AddClase(panelClases, CreateClassContainer("Hunter", new (string, string)[]
            {
                ("Hunter Beast Mastery", "ms-appx:///Assets/Hunter_BM.png"),
                ("Hunter Marksmanship", "ms-appx:///Assets/Hunter_MM.png"),
                ("Hunter Survival", "ms-appx:///Assets/Hunter_Survival.png")
            }));

            // Rogue
            AddClase(panelClases2, CreateClassContainer("Rogue", new (string, string)[]
            {
                ("Rogue Assassination", "ms-appx:///Assets/Rogue_Assassination.png"),
                ("Rogue Combat", "ms-appx:///Assets/Rogue_Combat.png"),
                ("Rogue Subtlety", "ms-appx:///Assets/Rogue_Subtlety.png")
            }));

            // Priest
            AddClase(panelClases2, CreateClassContainer("Priest", new (string, string)[]
            {
                ("Priest Discipline", "ms-appx:///Assets/Priest_Discipline.png"),
                ("Priest Holy", "ms-appx:///Assets/Priest_Holy.png"),
                ("Priest Shadow", "ms-appx:///Assets/Priest_Shadow.png")
            }));

            // Shaman
            AddClase(panelClases2, CreateClassContainer("Shaman", new (string, string)[]
            {
                ("Shaman Elemental", "ms-appx:///Assets/Shaman_Elemental.png"),
                ("Shaman Enhancement", "ms-appx:///Assets/Shaman_Enha.png"),
                ("Shaman Restoration", "ms-appx:///Assets/Shaman_Resto.png")
            }));

            // Mage
            AddClase(panelClases3, CreateClassContainer("Mage", new (string, string)[]
            {
                ("Mage Arcane", "ms-appx:///Assets/Mage_Arcane.png"),
                ("Mage Fire", "ms-appx:///Assets/Mage_Fire.png"),
                ("Mage Frost", "ms-appx:///Assets/Mage_Frost.png")
            }));

            // Warlock
            AddClase(panelClases3, CreateClassContainer("Warlock", new (string, string)[]
            {
                ("Warlock Affliction", "ms-appx:///Assets/Warlock_Affliction.png"),
                ("Warlock Demonology", "ms-appx:///Assets/Warlock_Demonology.png"),
                ("Warlock Destruction", "ms-appx:///Assets/Warlock_Destruction.png")
            }));

            // Druid
            AddClase(panelClases3, CreateClassContainer("Druid", new (string, string)[]
            {
                ("Druid Balance", "ms-appx:///Assets/Druid_Balance.png"),
                ("Druid Feral", "ms-appx:///Assets/Druid_Feral.png"),
                ("Druid Restoration", "ms-appx:///Assets/Druid_Resto.png")
            }));

            // StackPanel principal
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(inputBox);
            stackPanel.Children.Add(panelClases);
            stackPanel.Children.Add(panelClases2);
            stackPanel.Children.Add(panelClases3);

            // Dialog
            var dialog = new ContentDialog
            {
                Title = "Agregar Jugador",
                Content = stackPanel,
                PrimaryButtonText = "Guardar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary
                && !string.IsNullOrWhiteSpace(inputBox.Text)
                && !string.IsNullOrWhiteSpace(rolSeleccionado)
                && !string.IsNullOrWhiteSpace(rolIconoSeleccionado))
            {
                return new Player
                {
                    Name = inputBox.Text.Trim(),
                    Role = rolSeleccionado,
                    IconPath = rolIconoSeleccionado
                };
            }

            return null;
        }

        private StackPanel CreateClassContainer(string className, (string rol, string img)[] roles)
        {
            var container = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0)
            };

            container.Children.Add(new TextBlock
            {
                Text = className,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var panelRoles = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0)
            };

            foreach (var (rol, img) in roles)
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(img)),
                    Width = 30,
                    Height = 30
                };

                var btn = new Button
                {
                    Content = image,
                    Margin = new Thickness(2),
                    Padding = new Thickness(2)
                };

                btn.Click += (s, e) => SeleccionarRol(btn, rol, img, panelRoles);

                panelRoles.Children.Add(btn);
            }

            container.Children.Add(panelRoles);
            return container;
        }

        private void SeleccionarRol(Button btn, string rol, string icono, StackPanel panelRoles)
        {
            // Desmarcar el botón anterior
            if (botonSeleccionado != null)
                botonSeleccionado.ClearValue(Button.BackgroundProperty);

            // Marcar el nuevo
            btn.Background = new SolidColorBrush(Colors.LightBlue);

            // Actualizar datos
            botonSeleccionado = btn;
            rolSeleccionado = rol;
            rolIconoSeleccionado = icono; // ⚡ guardar icono
        }

        private void AddClase(StackPanel panelClases, StackPanel claseContainer)
        {
            panelClases.Children.Add(claseContainer);
        }

        public async Task<bool> ShowDeletePlayerConfirmationAsync(XamlRoot xamlRoot, Player player)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirmar eliminación",
                Content = $"¿Estás seguro de que deseas eliminar al jugador {player.Name}?",
                PrimaryButtonText = "Eliminar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = xamlRoot
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}