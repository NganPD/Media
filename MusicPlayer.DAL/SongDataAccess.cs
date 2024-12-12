using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MusicPlayer.DAL
{
    public class SongDataAccess
    {
        public async Task<List<Song>> LoadSongsAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("JSON file not found!");

            string jsonData = await File.ReadAllTextAsync(filePath);
            var songData = JsonConvert.DeserializeObject<SongData>(jsonData);
            return songData?.Songs ?? new List<Song>();
        }

        public async Task SaveSongsAsync(string filePath, List<Song> songs)
        {
            var songData = new SongData { Songs = songs };
            string jsonData = JsonConvert.SerializeObject(songData, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, jsonData);
        }
    }

    public class SongData
    {
        public List<Song> Songs { get; set; }
    }
}
