using Microsoft.UI.Xaml;
using RaidTeam.Models;
using System.Threading.Tasks;

namespace RaidTeam.Services
{
    public interface IDialogService
    {
        Task<Player?> ShowAddPlayerDialogAsync(XamlRoot xamlRoot);

        Task<bool> ShowDeletePlayerConfirmationAsync(XamlRoot xamlRoot, Player player);

        Task<string?> ShowEditRaidNameDialogAsync(XamlRoot xamlRoot, string currentName);
    }
}