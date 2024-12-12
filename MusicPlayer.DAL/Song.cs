namespace MusicPlayer.DAL
{
    public class Song
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Album { get; set; }
        public string Artist { get; set; } = null!;
        public string Source { get; set; } = null!;
        public string? Image { get; set; }
        public int Duration { get; set; } // thời gian bài hát (giây)
        public bool Favorite { get; set; }
        public int Counter { get; set; } // số lần phát
    }
}
    