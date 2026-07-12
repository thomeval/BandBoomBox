using System.Collections.Generic;

public class SongCreditsArtist
{
    public string Name { get; set; }
    public string WebsiteUrl { get; set; }
    public string ExtraUrl { get; set; }
    public SongCreditsUrlType ExtraUrlType { get; set; }
    public List<string> Songs { get; set; } = new List<string>();
}