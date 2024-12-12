using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using MusicPlayer.BLL;
using MusicPlayer.DAL;

namespace MusicPlayer.GUI
{
    public partial class MainWindow : Window
    {
        private List<Song> _songs;
        private SongManager _songManager;
        private CancellationTokenSource _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() => LoadSongsAsync());
        }
        private async Task LoadSongsAsync()
        {
            //string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Songs.json");
            string jsonFilePath = @"Assets/Song.json";
            try
            {
                // Kiểm tra sự tồn tại của tệp JSON
                if (!File.Exists(jsonFilePath))
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("The Songs.json file was not found! Please ensure it is in the 'Assets' folder.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });

                    return;
                }

                // Đọc tệp JSON
                var dal = new SongDataAccess();
                _songs = await dal.LoadSongsAsync(jsonFilePath);
                _songManager = new SongManager(_songs);

                // Cập nhật giao diện trên UI thread
                Dispatcher.Invoke(() =>
                {
                    SongsList.ItemsSource = _songs;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Failed to load songs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Select a JSON file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    var dal = new SongDataAccess();
                    _songs = await dal.LoadSongsAsync(filePath);
                    _songManager = new SongManager(_songs);

                    // Cập nhật giao diện trên UI thread
                    Dispatcher.Invoke(() =>
                    {
                        SongsList.ItemsSource = _songs;
                    });

                    MessageBox.Show($"Songs loaded successfully from: {filePath}", "Load Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("The selected file could not be found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the file. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void SongsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SongsList.SelectedItem is Song selectedSong)
            {
                // Nếu PlayerWindow đang mở, dừng phát nhạc trước
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is PlayerWindow openPlayerWindow) // Đổi tên biến ở đây
                    {
                        openPlayerWindow.StopPlayback(); // Gọi hàm StopPlayback từ PlayerWindow
                        openPlayerWindow.Close(); // Đóng cửa sổ PlayerWindow hiện tại
                    }
                }

                // Mở PlayerWindow mới
                var playerWindow = new PlayerWindow(selectedSong, _songs, false);
                playerWindow.Show();
            }
        }



        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // Thực hiện tìm kiếm khi nhấn phím Enter
            {
                SearchSongs();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchSongs();
        }

        private void SearchSongs()
        {
            string query = SearchBox.Text.Trim().ToLower();
            var searchCriteria = (SearchCriteriaComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(query))
            {
                SongsList.ItemsSource = _songs; // Hiển thị tất cả
                return;
            }

            List<Song> filteredSongs = new();

            switch (searchCriteria)
            {
                case "Title":
                    filteredSongs = _songs.Where(song =>
                        song.Title.ToLower().Contains(query)).ToList();
                    break;
                case "Artist":
                    filteredSongs = _songs.Where(song =>
                        song.Artist.ToLower().Contains(query)).ToList();
                    break;
                case "Album":
                    filteredSongs = _songs.Where(song =>
                        song.Album?.ToLower().Contains(query) ?? false).ToList();
                    break;
                default:
                    MessageBox.Show("Please select a valid search criteria.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
            }

            SongsList.ItemsSource = filteredSongs;

            if (!filteredSongs.Any())
            {
                MessageBox.Show("No matching results found.", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void BtnArtists_Click(object sender, RoutedEventArgs e)
        {
            var artists = _songManager.GetArtists();
            var selectionWindow = new SelectionWindow(artists);

            if (selectionWindow.ShowDialog() == true && selectionWindow.SelectedItems.Any())
            {
                var selectedArtists = selectionWindow.SelectedItems;
                var filteredSongs = _songs.Where(song => selectedArtists.Contains(song.Artist)).ToList();

                SongsList.ItemsSource = filteredSongs;

                if (!filteredSongs.Any())
                {
                    MessageBox.Show("No songs found for the selected artist(s).", "Filter Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnAlbums_Click(object sender, RoutedEventArgs e)
        {
            var albums = _songManager.GetAlbums();
            var selectionWindow = new SelectionWindow(albums);

            if (selectionWindow.ShowDialog() == true && selectionWindow.SelectedItems.Any())
            {
                var selectedAlbums = selectionWindow.SelectedItems;
                var filteredSongs = _songs.Where(song => selectedAlbums.Contains(song.Album)).ToList();

                SongsList.ItemsSource = filteredSongs;

                if (!filteredSongs.Any())
                {
                    MessageBox.Show("No songs found for the selected album(s).", "Filter Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 files (*.mp3)|*.mp3"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;

                // Tạo đối tượng Song cho file import
                var importedSong = new DAL.Song
                {
                    Title = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    Source = filePath,
                    Artist = "Unknown Artist"
                };

                // Mở PlayerWindow với trạng thái isFromImport = true
                var playerWindow = new PlayerWindow(importedSong, new List<DAL.Song> { importedSong }, true);
                playerWindow.Show();
            }
        }
    }
}
