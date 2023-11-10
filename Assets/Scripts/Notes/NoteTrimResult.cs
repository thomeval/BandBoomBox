using System.Collections.Generic;

public class NoteTrimResult
{
    public List<Note> NotesToRemove { get; set; } = new();
    public List<BeatLine> BeatLinesToRemove { get; set; } = new();
    public List<RegionMarker> RegionMarkersToRemove { get; set; } = new();
}
