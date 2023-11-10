using System;

public class NoteTrimmer
{
    public NoteTrimmer(NoteManager noteManager)
    {
        this.NoteManager = noteManager;
    }

    public float MissCutoff = 0.3f;
    public float BeatLineCutoff = 1.0f;
    public float RegionMarkerCutoff = 1.0f;

    public NoteManager NoteManager;

    public NoteTrimResult GetNotesToRemove()
    {
        var result = new NoteTrimResult();

        foreach (var note in NoteManager.Notes)
        {
            if (IsNoteExpired(note))
            {
                result.NotesToRemove.Add(note);
            }
            else
            {
                // Notes are ordered by AbsoluteTime. Stop processing once the current note is past the cutoff range.
                break;
            }
        }

        foreach (var beatLine in NoteManager.BeatLines)
        {
            if (IsNoteExpired(beatLine))
            {
                result.BeatLinesToRemove.Add(beatLine);
            }
            else
            {
                // BeatLines are ordered by AbsoluteTime. Stop processing once the current beatline is past the cutoff range.
                break;
            }
        }

        foreach (var marker in NoteManager.RegionMarkers)
        {
            if (IsRegionExpired(marker))
            {
                result.RegionMarkersToRemove.Add(marker);
            }
            else
            {
                // Region Markers are ordered by AbsoluteTime. Stop processing once the current marker is past the cutoff range.
                break;
            }
        }

        return result;
    }

    public bool IsNoteExpired(Note note)
    {
        return note.AbsoluteTime < NoteManager.SongPosition - this.MissCutoff;
    }

    public bool IsNoteExpired(BeatLine beatLine)
    {
        return beatLine.AbsoluteTime < NoteManager.SongPosition - this.BeatLineCutoff &&
               beatLine.BeatLineType != BeatLineType.Finish;
    }

    public bool IsRegionExpired(RegionMarker marker)
    {
        return marker.EndAbsoluteTime < NoteManager.SongPosition - this.RegionMarkerCutoff &&
               marker.RegionMarkerType != RegionMarkerType.SelectedRegion;
    }

    public void TrimNotes()
    {
        if (!NoteManager.TrimNotesEnabled)
        {
            return;
        }

        var notesToTrim = GetNotesToRemove();
        foreach (var note in notesToTrim.NotesToRemove)
        {
            NoteManager.ApplyNoteMissed(note);
        }

        foreach (var beatLine in notesToTrim.BeatLinesToRemove)
        {
            NoteManager.RemoveBeatLine(beatLine);
        }

        foreach (var regionMarker in notesToTrim.RegionMarkersToRemove)
        {
            NoteManager.RemoveRegionMarker(regionMarker);
        }
    }
}
