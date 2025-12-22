using System;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class HeldNoteDisplay : MonoBehaviour
{
    private const int LANE_COUNT = 3;
    public int ImpactZoneCenter = -540;
    public float NoteScale = 0.75f;
    public int LaneHeight = 100;

    public GameObject[] Lanes = new GameObject[LANE_COUNT];

    private SpriteResolver[] _laneResolvers;
    private SpriteRenderer[] _laneRenderers;

    [SerializeField]
    private Note[] _heldReleaseNotes = new Note[LANE_COUNT];

    void Awake()
    {
        _laneRenderers = new SpriteRenderer[LANE_COUNT];
        _laneResolvers = new SpriteResolver[LANE_COUNT];

        for (int x = 0; x < LANE_COUNT; x++)
        {
            _laneRenderers[x] = Lanes[x].GetComponent<SpriteRenderer>();
            _laneResolvers[x] = Lanes[x].GetComponent<SpriteResolver>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (_heldReleaseNotes.All(e => e == null))
        {
            this.gameObject.SetActive(false);
            return;
        }

        for (int x = 0; x < LANE_COUNT; x++)
        {
            if (_heldReleaseNotes[x] == null)
            {
                Lanes[x].SetActive(false);
                continue;
            }

            CalculateTailWidth(x);
        }
    }

    public void DisplayHeldNote(Note releaseNote)
    {
        if (releaseNote.NoteClass != NoteClass.Release)
        {
            throw new ArgumentException(
                $"Only Release notes are allowed to be used as hold targets in HeldNoteDisplay.");
        }
        var lane = releaseNote.Lane;

        this.gameObject.SetActive(true);
        Lanes[lane].SetActive(true);

        _heldReleaseNotes[lane] = releaseNote;
        _laneResolvers[lane].SetCategoryAndLabel("HoldTailsLit", releaseNote.NoteType.ToString());

        CalculateTailWidth(lane);

    }

    private void CalculateTailWidth(int lane)
    {
        var width = Math.Max(0.0f, _heldReleaseNotes[lane].transform.localPosition.x - ImpactZoneCenter);
        width /= NoteScale;

        _laneRenderers[lane].size = new Vector2(width, _laneRenderers[lane].size.y);

        var yPos = _laneRenderers[lane].transform.localPosition.y;
        _laneRenderers[lane].transform.localPosition = new Vector2(width / 2, yPos);
    }

    public void SetLaneOrder(LaneOrderType laneOrderType)
    {
        LaneOrderProvider.SetObjectLaneOrder(Lanes, laneOrderType, LaneHeight);
        //LaneOrderProvider.SetObjectLaneOrder(_heldReleaseNotes, laneOrderType);
    }
}
