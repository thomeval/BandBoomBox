using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class NoteManager : MonoBehaviour
{
    public List<Note> Notes;
    public List<BeatLine> BeatLines;

    public int ImpactZoneCenter = -540;

    public int LaneHeight = 100;
    public int TopLanePos  = 100;

    public float EarlyHitCutoff = -0.5f;
    public float LateHitCutoff = 0.3f;
    public float EarlyWrongCutoff = -2.0f;
    public float LateWrongCutoff = 1.0f;
    public float SongPosition = -999.0f;

    public float Offset;
    public float DEFAULT_VISIBILITY_RANGE = 8.0f;

    public string NoteSkin;
    public string LabelSkin;

    public float ScrollSpeed = 500;
    public int Slot = 1;

    private float _displayedScrollSpeed = 500;
    private readonly List<Note> _notesToRemove = new List<Note>();
    private readonly List<BeatLine> _beatLinesToRemove = new List<BeatLine>();

    private GameplayManager _gameplayManager;

    private readonly Note[] _pendingReleases = new Note [4];

    public float MaxVisiblePosition
    {
        get
        {
            if (_displayedScrollSpeed == 0.0f)
            {
                return 9999.0f;
            }

            var range = DEFAULT_VISIBILITY_RANGE * 500 / _displayedScrollSpeed;
            return SongPosition + range;
        }
    }

    public int MaxPerfPoints
    {
        get { return Notes.Count * HitJudge.JudgePerfPointValues[JudgeResult.Perfect]; }
    }

    public bool ParentEnabled 
    {
        get { return this.transform.parent.gameObject.activeSelf; }
        set
        {
            this.transform.parent.gameObject.SetActive(value);
        }
    }

    public void CalculateAbsoluteTimes(float bpm)
    {
        NoteUtils.CalculateAbsoluteTimes(this.Notes, bpm);
        NoteUtils.CalculateAbsoluteTimes(this.BeatLines, bpm);

        this.Notes = this.Notes.OrderBy(e => e.AbsoluteTime).ToList();
        this.BeatLines = this.BeatLines.OrderBy(e => e.AbsoluteTime).ToList();
    }

    // Start is called before the first frame update

    void Start()
    {
        _gameplayManager = GameObject.FindObjectOfType<GameplayManager>();

        if (Notes == null)
        {
            Notes = new List<Note>();
        }

    }

    void FixedUpdate()
    {
        bool speedChanged = UpdateScrollSpeed();

        if (!speedChanged)
        {
            return;
        }

        foreach (var holdNote in Notes.Where(e => e.NoteClass == NoteClass.Hold))
        {
            holdNote.CalculateTailWidth();
        }
    }

    public void UpdateNotes()
    {

        foreach (var note in Notes)
        {
            if (note.AbsoluteTime > this.MaxVisiblePosition)
            {
                break;
            }

            if (note.NoteClass == NoteClass.Hold)
            {
                note.CalculateTailWidth();
            }

            var xPos = CalculateRenderPosition(note.AbsoluteTime);
            var yPos = TopLanePos - (note.Lane * LaneHeight);
            note.SetPosition(xPos, yPos);
        }

        foreach (var beatLine in BeatLines)
        {
            if (beatLine.AbsoluteTime > this.MaxVisiblePosition)
            {
                break;
            }
            var xPos = CalculateRenderPosition(beatLine.AbsoluteTime);
            beatLine.SetPosition(xPos);
        }

        TrimNotes();
    }

    private bool UpdateScrollSpeed()
    {
        var diff = ScrollSpeed - _displayedScrollSpeed;

        if (diff == 0.0f)
        {
            return false;
        }

        if (Math.Abs(diff) < 1)
        {
            _displayedScrollSpeed = ScrollSpeed;

        }
        else
        {
            _displayedScrollSpeed += (diff / 5);
        }

        return true;
    }

    public void TrimNotes()
    {

        GetNotesToRemove();
        foreach (var note in _notesToRemove)
        {
            ApplyNoteMissed(note);
        }

        foreach (var beatLine in _beatLinesToRemove)
        {
            RemoveBeatLine(beatLine);
        }
    }

    private void GetNotesToRemove()
    {
        _notesToRemove.Clear();
        _beatLinesToRemove.Clear();
        foreach (var note in this.Notes)
        {
            if (IsNoteExpired(note))
            {
                _notesToRemove.Add(note);
            }
            else
            {
                // Notes are ordered by AbsoluteTime. Stop processing once the current note is inside the cutoff range.
                break;
            }
        }

        foreach (var beatLine in this.BeatLines)
        {
            if (IsNoteExpired(beatLine))
            {
                _beatLinesToRemove.Add(beatLine);
            }
            else
            {
                // BeatLines are ordered by AbsoluteTime. Stop processing once the current beatline is inside the cutoff range.
                break;
            }
        }
    }

    private bool IsNoteExpired(Note note)
    {
        return note.AbsoluteTime < SongPosition - NoteUtils.MissCutoff;
    }

    private bool IsNoteExpired(BeatLine beatLine)
    {
        return beatLine.AbsoluteTime < SongPosition - NoteUtils.BeatLineCutoff &&
               beatLine.BeatLineType != BeatLineType.Finish;
    }

    private float CalculateRenderPosition(float notePosition)
    {
        float newX = ((notePosition - SongPosition) * _displayedScrollSpeed) + ImpactZoneCenter;
        return newX;
    }

    private IEnumerable<Note> EnforceCutoffs(IEnumerable<Note> notes, float earlyCutoff, float lateCutoff)
    {
        return notes.Where(e => e.AbsoluteTime >= earlyCutoff && e.AbsoluteTime <= lateCutoff);
    }

    /// <summary>
    /// Finds the earliest note of the given NoteType (or its corresponding 'Any' type) that exists in the Note Highway.
    /// If enforceCutoffs is set to true, only Notes within the EarlyCutoff and LateCutoff range (relative to current song position) are considered.
    /// </summary>
    /// <param name="noteType">The type of note to search for. </param>
    /// <param name="enforceCutoffs">Whether to respect the EarlyCutoff and LateCutoff range. If set to false, the earliest matching note is returned regardless of its position.</param>
    /// <returns>The earliest matching note that exists in the Note Highway.</returns>
    public Note FindNextNote(NoteType noteType, bool enforceCutoffs)
    {
        var laneAnyType = NoteUtils.GetLaneAnyNote(noteType);
        IEnumerable<Note> notes = Notes.Where(e => e.NoteType == noteType || e.NoteType == laneAnyType).Where(e => e.NoteClass == NoteClass.Tap || e.NoteClass == NoteClass.Hold).OrderBy(e => e.AbsoluteTime);

        if (enforceCutoffs)
        {
            var earlyCutoff = SongPosition + EarlyHitCutoff;
            var lateCutoff = SongPosition + LateHitCutoff;
            notes = EnforceCutoffs(notes, earlyCutoff, lateCutoff);
        }

        notes = notes.ToList();
        return notes.FirstOrDefault();
    }

    /// <summary>
    /// Finds the earliest note that exists in the Note Highway.
    /// If enforceCutoffs is set to true, only Notes within the EarlyCutoff and LateCutoff range (relative to current song position) are considered.
    /// </summary>
    /// <param name="enforceCutoffs">Whether to respect the EarlyCutoff and LateCutoff range. If set to false, the earliest note is returned regardless of its position.</param>
    /// <returns>The earliest note that exists in the Note Highway.</returns>
    public Note FindNextNote(bool enforceCutoffs)
    {
        IEnumerable<Note> notes = Notes.Where(e => e.NoteClass == NoteClass.Tap || e.NoteClass == NoteClass.Hold).OrderBy(e => e.AbsoluteTime);

        if (enforceCutoffs)
        {
            var earlyCutoff = SongPosition + EarlyWrongCutoff;
            var lateCutoff = SongPosition + LateWrongCutoff;
            notes = EnforceCutoffs(notes, earlyCutoff,lateCutoff);
        }

        notes = notes.ToList();
        return notes.FirstOrDefault();
    }

    public void OnNoteHeld(int lane)
    {
        var pending = FindNextRelease(lane);
        _pendingReleases[lane] = pending;
    }

    public Note OnNoteReleased(int lane)
    {
        var temp = _pendingReleases[lane];
        _pendingReleases[lane] = null;
        if (temp != null)
        {
            RemoveNote(temp);
        }

        return temp;
    }

    public Note FindNextRelease(int lane)
    {
        var result = Notes.FirstOrDefault(e => e.NoteClass == NoteClass.Release && e.Lane == lane);

        if (result == null)
        {
            throw new ArgumentException($"Missing Hold Release for lane {lane}!");
        }
        return result;
    }

    public void ApplyNoteMissed(Note note)
    {
        if (note.EndNote != null)
        {
            ApplyNoteMissed(note.EndNote);
        }

        _gameplayManager?.OnNoteMissed(note, this.Slot);
        RemoveNote(note);
        
    }
    public void RemoveNote(Note note)
    {

        if (note.NoteClass == NoteClass.Release)
        {
            _pendingReleases[note.Lane] = null;
        }

        Notes.Remove(note);
        GameObject.Destroy(note.gameObject);
    }

    public void RemoveBeatLine(BeatLine beatLine)
    {
        BeatLines.Remove(beatLine);
        GameObject.Destroy(beatLine.gameObject);
    }

    public void ApplyNoteSkin(string noteSkin = null, string labelSkin = null)
    {
        if (noteSkin != null)
        {
            NoteSkin = noteSkin;
        }

        if (labelSkin != null)
        {
            LabelSkin = labelSkin;
        }

        foreach (var note in Notes)
        {
            note.SetSpriteCategories(this.NoteSkin, this.LabelSkin);
        }
        foreach (var beatLine in BeatLines)
        {
            beatLine.SetSprite();
        }

    }

    public void ClearNotes()
    {
        for (int x = 0; x < _pendingReleases.Length; x++)
        {
            _pendingReleases[x] = null;
        }
        foreach (var note in this.Notes)
        {
            GameObject.Destroy(note.gameObject);
        }

        foreach (var beatline in this.BeatLines)
        {
            GameObject.Destroy(beatline.gameObject);
        }

        this.BeatLines.Clear();
        this.Notes.Clear();
    }

    public void AttachNotes()
    {
        foreach (var note in this.Notes)
        {
            note.transform.SetParent(this.transform, false);
            note.transform.localPosition = new Vector3(9999.0f, note.transform.localPosition.y);
        }
    }
    public void AttachBeatLines()
    {
        foreach (var beatLine in this.BeatLines)
        {
            beatLine.transform.SetParent(this.transform, false);
            beatLine.transform.localPosition = new Vector3(9999.0f, beatLine.transform.localPosition.y);
        }
    }
}
