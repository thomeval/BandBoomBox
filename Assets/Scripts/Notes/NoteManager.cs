using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class NoteManager : MonoBehaviour
{
    public List<Note> Notes = new();
    public List<BeatLine> BeatLines = new();
    public List<RegionMarker> RegionMarkers = new();
    public SongChart Chart;
    public RectTransform ScrollingBackground;
    public RegionMarker SelectedRegionMarker
    {
        get
        {
            return this.RegionMarkers.FirstOrDefault(e => e.RegionMarkerType == RegionMarkerType.SelectedRegion);
        }
    }

    public int ImpactZoneCenter = -540;

    public int LaneHeight = 100;
    public int TopLanePos = 100;

    public float EarlyHitCutoff = -0.5f;
    public float LateHitCutoff = 0.3f;
    public float EarlyWrongCutoff = -2.0f;
    public float LateWrongCutoff = 1.0f;

    /// <summary>
    /// Gets or sets the current position of the highway, in seconds, relative to the song's offset point. 
    /// </summary>
    public float SongPosition = -999.0f;

    public float SongPositionInBeats = 0.0f;

    public float DEFAULT_VISIBILITY_RANGE = 8.0f;

    public string NoteSkin;
    public string LabelSkin;

    public int ScrollSpeed = 500;
    public int Slot = 1;
    public bool TrimNotesEnabled = true;

    private float _displayedScrollSpeed = 500;
    private float _lastSeenPosition = -999.0f;
    private float _noteAreaWidth;

    private GameplayManager _gameplayManager;
    private NoteTrimmer _noteTrimmer;
    private SpriteRenderer _scrollingBackgroundRenderer;

    private readonly Note[] _pendingReleases = new Note[4];

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

    /// <summary>
    /// Gets or sets whether the Note Highway Gameobject is enabled.
    /// </summary>
    public bool ParentEnabled
    {
        get { return this.transform.parent.parent.gameObject.activeSelf; }
        set
        {
            this.transform.parent.parent.gameObject.SetActive(value);
        }
    }

    [SerializeField]
    private float _scrollingBackgroundOpacity;

    public float ScrollingBackgroundOpacity
    {
        get
        {
            return _scrollingBackgroundOpacity;
        }
        set
        {
            value = Mathf.Clamp01(value);
            _scrollingBackgroundOpacity = value;
        }
    }

    [field: SerializeField]
    public bool TurboActive { get; set; }

    public void CalculateAbsoluteTimes(float bpm)
    {
        NoteUtils.CalculateAbsoluteTimes(this.Notes, bpm);
        NoteUtils.CalculateAbsoluteTimes(this.BeatLines, bpm);
        NoteUtils.CalculateAbsoluteTimes(this.RegionMarkers, bpm);

        this.Notes = this.Notes.OrderBy(e => e.AbsoluteTime).ToList();
        this.BeatLines = this.BeatLines.OrderBy(e => e.AbsoluteTime).ToList();
        this.RegionMarkers = this.RegionMarkers.OrderBy(e => e.StartAbsoluteTime).ToList();
    }

    public void SetNoteMxValue()
    {
        if (this.Chart == null)
        {
            return;
        }

        var mxFromHit = HitJudge.JudgeMxValues[JudgeResult.Perfect] * HitJudge.DifficultyMxValues[this.Chart.Difficulty];

        foreach (var note in this.Notes)
        {
            note.MxValue = mxFromHit;
        }
    }

    void Awake()
    {
        _gameplayManager = GameObject.FindObjectOfType<GameplayManager>();
    }

    void FixedUpdate()
    {

        if (_lastSeenPosition > SongPosition)
        {
            HideAllNotes();
        }

        _lastSeenPosition = SongPosition;
        UpdateBackgroundOpacity();

        bool speedChanged = UpdateScrollSpeed();

        if (!speedChanged)
        {
            return;
        }

        foreach (var holdNote in Notes.Where(e => e.NoteClass == NoteClass.Hold && e.AbsoluteTime < MaxVisiblePosition))
        {
            holdNote.CalculateTailWidth();
        }
    }

    private const float SCROLLING_BACKGROUND_SHOW_SPEED = 0.015f;
    private const float SCROLLING_BACKGROUND_HIDE_SPEED = 0.01f;
    private const float MIN_BACKGROUND_OPACITY = 0.25f;

    private void UpdateBackgroundOpacity()
    {
        if (TurboActive)
        {
            ScrollingBackgroundOpacity += SCROLLING_BACKGROUND_SHOW_SPEED;
        }
        else
        {
            ScrollingBackgroundOpacity -= SCROLLING_BACKGROUND_HIDE_SPEED;
        }

        _scrollingBackgroundRenderer ??= ScrollingBackground.GetComponent<SpriteRenderer>();

        var opacity = CalculateBackgroundOpacity(ScrollingBackgroundOpacity, SongPositionInBeats);
        _scrollingBackgroundRenderer.color = new Color(1.0f, 1.0f, 1.0f, opacity);
    }

    private float CalculateBackgroundOpacity(float scrollingBackgroundOpacity, float songPositionInBeats)
    {
        var beatFraction = songPositionInBeats - (int)songPositionInBeats;
        beatFraction = 1 - beatFraction;

        var result = MIN_BACKGROUND_OPACITY;

        var lerp = (1 - MIN_BACKGROUND_OPACITY) * beatFraction;
        result += lerp;
        result *= scrollingBackgroundOpacity;
        return result;

    }

    private void UpdateBackground()
    {

        if (_noteAreaWidth == 0)
        {
            _noteAreaWidth = ScrollingBackground.rect.width;
        }

        var newX = (this._displayedScrollSpeed * this.SongPosition * -1) % _noteAreaWidth;

        if (float.IsNaN(newX))
        {
            return;
        }

        ScrollingBackground.position = new Vector3(newX, ScrollingBackground.position.y);

        // var speedScale = this._displayedScrollSpeed / 500.0f;
        // ScrollingBackground.localScale = new Vector3(speedScale, 1.0f);
    }

    public void HideAllNotes()
    {
        foreach (var note in Notes)
        {
            note.SetRenderXPosition(10000.0f);
        }
        foreach (var beatline in BeatLines)
        {
            beatline.SetRenderPosition(10000.0f);
        }

        foreach (var marker in RegionMarkers)
        {
            marker.SetRenderPosition(10000.0f, 10001.0f);
        }
    }

    public void ShowAllNotes()
    {
        foreach (var note in Notes)
        {
            note.SetRenderXPosition(CalculateRenderPosition(note.AbsoluteTime));
        }
        foreach (var beatline in BeatLines)
        {
            beatline.SetRenderPosition(CalculateRenderPosition(beatline.AbsoluteTime));
        }
        foreach (var region in RegionMarkers)
        {
            var startPos = CalculateRenderPosition(region.StartAbsoluteTime);
            var endPos = CalculateRenderPosition(region.EndAbsoluteTime);
            region.SetRenderPosition(startPos, endPos);
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
            note.SetRenderPosition(xPos, yPos);
        }

        foreach (var beatLine in BeatLines)
        {
            if (beatLine.AbsoluteTime > this.MaxVisiblePosition)
            {
                break;
            }
            var xPos = CalculateRenderPosition(beatLine.AbsoluteTime);
            beatLine.SetRenderPosition(xPos);
        }

        foreach (var region in RegionMarkers)
        {
            SetRenderPosition(region);
        }

        if (SelectedRegionMarker != null)
        {
            SetRenderPosition(SelectedRegionMarker);
        }

        TrimNotes();

        UpdateBackground();
    }

    private void SetRenderPosition(RegionMarker region)
    {
        if (region.StartAbsoluteTime > this.MaxVisiblePosition)
        {
            return;
        }
        var startPos = CalculateRenderPosition(region.StartAbsoluteTime);
        var endPos = CalculateRenderPosition(region.EndAbsoluteTime);

        if (float.IsNaN(startPos) || float.IsNaN(endPos))
        {
            return;
        }
        region.SetRenderPosition(startPos, endPos);
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
        if (_noteTrimmer == null)
        {
            _noteTrimmer = new NoteTrimmer(this);
        }
        _noteTrimmer.TrimNotes();
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
            notes = EnforceCutoffs(notes, earlyCutoff, lateCutoff);
        }

        notes = notes.ToList();
        return notes.FirstOrDefault();
    }

    public Note FindNearestNote(NoteType noteType, bool enforceCutoffs = true)
    {
        var noteAnyType = NoteUtils.GetLaneAnyNote(noteType);
        var result = Notes.OrderBy(e => e.DistanceTo(SongPosition))
            .FirstOrDefault(e => e.NoteType == noteType && e.NoteClass != NoteClass.Release);

        if (enforceCutoffs)
        {
            result = EnforceCutoffs(result);
        }

        return result;
    }

    private Note EnforceCutoffs(Note note)
    {
        var earlyCutoff = SongPosition + EarlyHitCutoff;
        var lateCutoff = SongPosition + LateHitCutoff;

        if (note == null)
        {
            return null;
        }

        if (note.AbsoluteTime < earlyCutoff || note.AbsoluteTime > lateCutoff)
        {
            return null;
        }

        return note;
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

    public Note FindNoteBefore(float position, int lane)
    {
        var result = Notes.LastOrDefault(e => e.Position < position && e.Lane == lane);
        return result;
    }

    public Note FindNoteAfter(float position, int lane)
    {
        var result = Notes.FirstOrDefault(e => e.Position > position && e.Lane == lane);
        return result;
    }

    public Note FindNearestRelease(int lane, bool enforceCutoffs = true)
    {
        var result = Notes.OrderBy(e => e.DistanceTo(SongPosition))
            .FirstOrDefault(e => e.Lane == lane && e.NoteClass == NoteClass.Release);

        if (enforceCutoffs)
        {
            result = EnforceCutoffs(result);
        }

        return result;
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
    public void RemoveNote(Note note, bool removeEnd = false)
    {

        if (note.NoteClass == NoteClass.Release)
        {
            _pendingReleases[note.Lane] = null;
        }

        if (removeEnd && note.EndNote != null)
        {
            Notes.Remove(note.EndNote);
            GameObject.Destroy(note.EndNote.gameObject);
        }

        Notes.Remove(note);
        GameObject.Destroy(note.gameObject);
    }

    public void RemoveBeatLine(BeatLine beatLine)
    {
        BeatLines.Remove(beatLine);
        GameObject.Destroy(beatLine.gameObject);
    }

    public void RemoveRegionMarker(RegionMarker marker)
    {
        RegionMarkers.Remove(marker);
        GameObject.Destroy(marker.gameObject);
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

    public void AttachNote(Note note)
    {
        if (!Notes.Contains(note))
        {
            Notes.Add(note);
        }

        note.transform.SetParent(this.transform, false);
        note.transform.localPosition = new Vector3(9999.0f, note.transform.localPosition.y);
        note.SetSpriteCategories(this.NoteSkin, this.LabelSkin);
        note.RefreshLane();
    }

    public void AttachNotes()
    {
        foreach (var note in this.Notes)
        {
            AttachNote(note);
        }

        Notes = Notes.OrderBy(e => e.Position).ToList();
        SetNoteMxValue();
    }

    public void AttachNotes(IEnumerable<Note> notes)
    {
        foreach (var note in notes)
        {
            AttachNote(note);
        }

        Notes = Notes.OrderBy(e => e.Position).ToList();
        SetNoteMxValue();
    }

    public void AttachBeatLines()
    {
        foreach (var beatLine in this.BeatLines)
        {
            beatLine.transform.SetParent(this.transform, false);
            beatLine.transform.localPosition = new Vector3(9999.0f, beatLine.transform.localPosition.y);
        }
    }
    public void AttachRegionMarkers()
    {
        foreach (var marker in this.RegionMarkers)
        {
            marker.transform.SetParent(this.transform, false);
            marker.transform.localPosition = new Vector3(9999.0f, marker.transform.localPosition.y);
        }
    }

    public Note GetNoteAtPosition(double position, int lane)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        var result = Notes.SingleOrDefault(e => (double)e.Position == position && e.Lane == lane);
        return result;
    }

    public List<Note> GetNotesAtPosition(double position)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        var result = Notes.Where(e => (double)e.Position == position).ToList();
        return result;
    }

    public Note FindReleaseNoteStart(Note releaseNote)
    {
        if (releaseNote.NoteClass != NoteClass.Release)
        {
            throw new ArgumentException("FindReleaseNoteStart can only be called for Release notes.");
        }

        return Notes.SingleOrDefault(e => e.EndNote == releaseNote);
    }

    public List<Note> GetNotesInRegion(double regionStart, double regionEnd)
    {
        var result = Notes.Where(e => e.Position >= regionStart && e.Position <= regionEnd).ToList();

        // If a hold note starts in the selected region but its release is outside, process the release note as well.
        foreach (var holdNote in result.Where(e => e.EndNote != null).ToList())
        {
            if (!result.Contains(holdNote.EndNote))
            {
                result.Add(holdNote.EndNote);
            }

        }

        // If a hold note ends in the selected region but its start is outside, don't process it.
        foreach (var releaseNote in result.Where(e => e.NoteClass == NoteClass.Release).ToList())
        {
            var match = result.SingleOrDefault(e => e.EndNote == releaseNote);

            if (match == null)
            {
                result.Remove(releaseNote);
            }
        }
        return result;
    }

    public void HighlightRegion(float? start, float? end, float bpm)
    {
        if (start == null || end == null)
        {
            start = 0;
            end = 0;
        }
        this.SelectedRegionMarker.StartPosition = start.Value;
        this.SelectedRegionMarker.EndPosition = end.Value;
        NoteUtils.CalculateAbsoluteTimes(this.SelectedRegionMarker, bpm);
        SetRenderPosition(this.SelectedRegionMarker);
    }


}
