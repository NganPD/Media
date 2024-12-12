using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NAudio.Wave;
using MahApps.Metro.IconPacks;


namespace MusicPlayer.GUI
{
    public partial class PlayerWindow : Window
    {
        private List<DAL.Song> _songs;
        private int _currentSongIndex;
        private bool _isFromImport;
        private IWavePlayer _waveOut;
        private AudioFileReader _audioFile;
        private DAL.Song currentSong;
        private DispatcherTimer _timer;
        private bool _isPlaying;

        public PlayerWindow(DAL.Song selectedSong, List<DAL.Song> songs, bool isFromImport = false)
        {
            InitializeComponent();

            _isFromImport = isFromImport;
            _songs = songs;
            _currentSongIndex = _songs.IndexOf(selectedSong);
            currentSong = selectedSong;

            BtnNext.Visibility = _isFromImport ? Visibility.Collapsed : Visibility.Visible;
            BtnPrevious.Visibility = _isFromImport ? Visibility.Collapsed : Visibility.Visible;

            AlbumCover.Source = !string.IsNullOrEmpty(currentSong.Image)
                ? new BitmapImage(new Uri(currentSong.Image, UriKind.Absolute))
                : new BitmapImage(new Uri("D:\\Media\\MusicPlayer.GUI\\Assets\\cover.jpg", UriKind.Absolute));

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += Timer_Tick;

            PlayCurrentSong();
        }

        private void PlayCurrentSong()
        {
            if (_currentSongIndex < 0 || _currentSongIndex >= _songs.Count)
                return;

            currentSong = _songs[_currentSongIndex];
            SongTitle.Text = currentSong.Title;
            ArtistName.Text = currentSong.Artist;
            Album.Text = currentSong.Album;

            // Cập nhật hình ảnh album
            AlbumCover.Source = !string.IsNullOrEmpty(currentSong.Image)
                ? new BitmapImage(new Uri(currentSong.Image, UriKind.Absolute))
                : new BitmapImage(new Uri("D:\\Media\\MusicPlayer.GUI\\Assets\\cover.jpg", UriKind.Absolute));

            StopPlayback();

            try
            {
                _audioFile = new AudioFileReader(currentSong.Source);
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_audioFile);
                _waveOut.Play();

                PlayPauseIcon.Kind = PackIconMaterialKind.Pause;

                PlaybackSlider.Maximum = _audioFile.TotalTime.TotalSeconds;
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot play the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public void StopPlayback()
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _audioFile?.Dispose();
            _waveOut = null;
            _audioFile = null;
            _timer.Stop();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_waveOut != null)
            {
                if (_waveOut.PlaybackState == PlaybackState.Playing)
                {
                    _waveOut.Pause();
                    PlayPauseIcon.Kind = PackIconMaterialKind.Play; // Set Play icon
                }
                else
                {
                    _waveOut.Play();
                    PlayPauseIcon.Kind = PackIconMaterialKind.Pause; // Set Pause icon
                }
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_audioFile != null)
            {
                PlaybackSlider.Value = _audioFile.CurrentTime.TotalSeconds;
            }
        }

        private void PlaybackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_audioFile != null && Math.Abs(_audioFile.CurrentTime.TotalSeconds - PlaybackSlider.Value) > 1)
            {
                _audioFile.CurrentTime = TimeSpan.FromSeconds(PlaybackSlider.Value);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            _currentSongIndex = (_currentSongIndex + 1) % _songs.Count;
            PlayCurrentSong();
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _currentSongIndex = (_currentSongIndex - 1 + _songs.Count) % _songs.Count;
            PlayCurrentSong();
        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            // Đặt vị trí của popup dựa vào nút Volume
            VolumePopup.PlacementTarget = BtnVolume;

            // Hiển thị hoặc đóng popup
            VolumePopup.IsOpen = !VolumePopup.IsOpen;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_waveOut != null)
            {
                // Điều chỉnh âm lượng (0.0 đến 1.0)
                _waveOut.Volume = (float)(VolumeSlider.Value / 100);
            }
        }



        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the playlist selection window
            var playlistWindow = new SelectionWindow(_songs.Select(s => s.Title).ToList());
            if (playlistWindow.ShowDialog() == true && playlistWindow.SelectedItems.Count > 0)
            {
                // Play the selected song
                var selectedSongTitle = playlistWindow.SelectedItems.First();
                _currentSongIndex = _songs.FindIndex(s => s.Title == selectedSongTitle);
                PlayCurrentSong();
            }
        }



        protected override void OnClosed(EventArgs e)
        {
            StopPlayback();
            base.OnClosed(e);
        }
    }
}
