# Music Player

Welcome to the Music Player project! This WPF application is designed to provide an engaging and feature-rich music playback experience, leveraging a layered architecture. The project is built using .NET and integrates the NAudio library for audio playback.

## Features

- **Music Playback**: Play your favorite songs seamlessly with high-quality audio output.
- **JSON-based Data Management**: Song data is stored in a `Song.json` file, which allows for easy addition and management of songs.
- **External MP3 Support**: Play MP3 files from outside the application by specifying their file paths.
- **Layered Architecture**: Designed using a layered architecture for scalability and maintainability.
- **NAudio Integration**: Leverages the NAudio library for advanced audio playback functionalities.
- **Image Support**: Displays album art using the URL stored in the `Song.Image` field.

## Project Structure

The repository is divided into the following key components:

1. **`.git`, `.gitattributes`, `.gitignore`**: Configuration files for version control.
2. **`.vs`**: Visual Studio-specific files.
3. **`Media.sln`**: The solution file for the project.
4. **`MusicPlayer.BLL`**: Contains the Business Logic Layer to handle core functionalities.
5. **`MusicPlayer.DAL`**: Includes the Data Access Layer for managing data-related operations.
6. **`MusicPlayer.GUI`**: The Graphical User Interface layer that serves as the front-end for the application.

## Prerequisites

To run this project, ensure you have the following installed:

- .NET Framework (version X.X or higher)
- Visual Studio (recommended for development)
- NAudio library

## Setup Instructions

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/yourusername/music-player.git
   ```

2. **Run the Application**:

   - Open the solution in Visual Studio.
   - Build the solution.
   - Start the application and explore the music player features.

## Usage

- **Adding Songs**: Add songs to the `Song.json` file with their file paths and album art URLs.
- **Playback Controls**: Play, pause, stop, and navigate through songs with intuitive controls.
- **View Album Art**: Display album art linked via the `Song.Image` field.

## Technologies Used

- **WPF**: For creating the desktop user interface.
- **NAudio**: For audio playback.
- **JSON**: For lightweight and flexible data storage.

## Contribution Guidelines

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new branch (`feature/your-feature-name`).
3. Commit your changes and push to the branch.
4. Open a pull request with a detailed description of your changes.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For any queries or suggestions, feel free to reach out:

- **Name**: Pham Dinh Ngan
- **Email**: Phamdinhngancc151@gmail.com
- **GitHub**: https://github.com/NganPD

Enjoy using the Music Player! 🎵

