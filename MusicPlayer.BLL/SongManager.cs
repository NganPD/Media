using System.Collections.Generic;
using System.Linq;
using MusicPlayer.DAL;

namespace MusicPlayer.BLL
{
    public class SongManager
    {
        private readonly List<Song> _songs;

        public SongManager(List<Song> songs)
        {
            _songs = songs;
        }

        public List<string> GetArtists()
        {
            return _songs.Select(s => s.Artist).Distinct().ToList();
        }

        public List<string?> GetAlbums() => _songs.Select(static s => s.Album).Distinct().ToList();

        public List<Song> GetSongsByArtist(string artist)
        {
            return _songs.Where(s => s.Artist == artist).ToList();
        }

        public List<Song> GetSongsByAlbum(string album)
        {
            return _songs.Where(s => s.Album == album).ToList();
        }
    }
}
