using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using RaidTeam.Data;
using RaidTeam.Repositories;
using RaidTeam.Services;
using RaidTeam.ViewModels;
using System;
using System.IO;

namespace RaidTeam
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }

        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();

            // Configurar EF Core con SQLite (archivo local en AppData)
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaidTeam.db");

            services.AddDbContext<RaidTeamDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Repositorios
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IRaidTeamRepository, RaidTeamRepository>();

            // Servicios
            services.AddSingleton<IDialogService, DialogService>();

            // ViewModels
            services.AddTransient<MainViewModel>();

            Services = services.BuildServiceProvider();

            // Crear DB si no existe
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RaidTeamDbContext>();
            // db.Database.EnsureDeleted(); // No borrar la base de datos en cada inicio
            db.Database.EnsureCreated();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var window = new MainWindow(
                Services.GetRequiredService<MainViewModel>(),
                Services.GetRequiredService<IDialogService>());
            window.Activate();
        }
    }
}