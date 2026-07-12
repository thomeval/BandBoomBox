using System.Collections.Generic;

public class SongCreditsSet
{
    public int Version { get; set; }
    public List<SongCreditsArtist> Artists { get; set; } = new();

}