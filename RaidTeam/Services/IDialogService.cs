using Microsoft.UI.Xaml;
using RaidTeam.Models;
using System.Threading.Tasks;

namespace RaidTeam.Services
{
    public interface IDialogService
    {
        Task<Player?> ShowAddPlayerDialogAsync(XamlRoot xamlRoot);
    }
}