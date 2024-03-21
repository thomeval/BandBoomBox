using UnityEngine;
using UnityEngine.UI;

public class SongChartNoteCountDisplay : MonoBehaviour
{
    public Text TxtTopLaneCount;
    public Text TxtMiddleLaneCount;
    public Text TxtBottomLaneCount;
    public Text TxtTapNoteCount;
    public Text TxtHoldNoteCount;
    public Text TxtTotalNoteCount;

    public void UpdateNoteCountDisplay(SongChartNoteCounts counts)
    {
        TxtTopLaneCount.text = counts.LaneNotes[0].ToString();
        TxtMiddleLaneCount.text = counts.LaneNotes[1].ToString();
        TxtBottomLaneCount.text = counts.LaneNotes[2].ToString();
        TxtTapNoteCount.text = counts.TapNotes.ToString();
        TxtHoldNoteCount.text = counts.HoldNotes.ToString();
        TxtTotalNoteCount.text = counts.TotalNotes.ToString();
    }

}
