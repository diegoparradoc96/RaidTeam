# WoW Raid Team Manager

A modern Windows desktop application built with WinUI 3 for managing World of Warcraft TBC raid teams. This tool helps raid leaders organize their teams efficiently with an intuitive drag-and-drop interface.

<img width="1414" height="702" alt="image" src="https://github.com/user-attachments/assets/0c4998f5-0cb7-436d-9d32-44ae28640609" />

## Features

- **Multiple Raid Teams Management**
  - Create and manage multiple raid team compositions
  - Easy switching between different raid teams
  - Customizable raid team names

- **Player Management**
  - Add players with their class and specialization
  - Visual class icons for easy identification
  - Support for all WoW classic TBC classes and specs:
    - Warriors (Arms, Fury, Protection)
    - Paladins (Holy, Protection, Retribution)
    - Hunters (Beast Mastery, Marksmanship, Survival)
    - Rogues (Assassination, Combat, Subtlety)
    - Priests (Discipline, Holy, Shadow)
    - Shamans (Elemental, Enhancement, Restoration)
    - Mages (Arcane, Fire, Frost)
    - Warlocks (Affliction, Demonology, Destruction)
    - Druids (Balance, Feral, Restoration)

- **Group Organization**
  - 5 main groups with 5 slots each
  - Additional bench group for reserve players
  - Drag-and-drop interface for easy player assignment
  - Visual feedback during drag operations

- **Filtering and Search**
  - Filter players by class/role
  - Search players by name
  - Clear and intuitive filtering interface

## Technical Details

- Built with .NET 8 and WinUI 3
- MVVM architecture using CommunityToolkit.Mvvm
- Local SQLite database for data persistence
- Dependency Injection using Microsoft.Extensions.DependencyInjection
- Entity Framework Core for database operations

## Requirements

- Windows 10 version 1809 or higher
- .NET 8.0 Runtime

## Installation

1. Download the latest release from the releases page
2. Run the MSIX installer
3. Launch the application

## Development

To set up the development environment:

1. Install Visual Studio 2022 or later
2. Install the "Windows App SDK" workload
3. Clone the repository
4. Open the solution in Visual Studio
5. Restore NuGet packages
6. Build and run

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
