using System.Collections.Generic;
using UnityEngine;

public static class LaneOrderProvider
{
    /// <summary>
    /// Returns the lane order array based on the specified LaneOrderType. 
    /// Indices: 0 = Triggers, 1 = Arrows, 2 = ABXY. Values: 0 = Top, 1 = Middle, 2 = Bottom.
    /// </summary>
    private static readonly Dictionary<LaneOrderType, int[]> _laneOrderMappings = new()
    {
        { LaneOrderType.Standard, new[] { 0, 1, 2 } },
        { LaneOrderType.BottomTrigger, new[] { 2, 0, 1 } },
        { LaneOrderType.MiddleTrigger, new[] { 1, 0, 2 } },
        { LaneOrderType.Flipped, new[] { 0, 2, 1 } }
    };

    public static int[] GetLaneOrder(LaneOrderType laneOrderType)
    {
        if (_laneOrderMappings.TryGetValue(laneOrderType, out var order))
        {
            return order;
        }
        return _laneOrderMappings[LaneOrderType.Standard]; // Default to standard order
    }

    public const int NOTE_LANE_HEIGHT = 75;

    public static void SetObjectLaneOrder(GameObject[] objects, LaneOrderType laneOrderType, int laneHeight = NOTE_LANE_HEIGHT)
    {
        var laneOrder = LaneOrderProvider.GetLaneOrder(laneOrderType);

        for (int x = 0; x < objects.Length; x++)
        {
            var existing = objects[x].transform.localPosition;
            var offset = laneHeight * laneOrder[x] * -1;

            objects[x].transform.localPosition = new Vector3(existing.x, offset + laneHeight, existing.z);
        }
    }

    public static void SetObjectLaneOrder(MonoBehaviour[] objects, LaneOrderType laneOrderType, int laneHeight = NOTE_LANE_HEIGHT)
    {
        var laneOrder = LaneOrderProvider.GetLaneOrder(laneOrderType);

        for (int x = 0; x < objects.Length; x++)
        {
            var existing = objects[x].transform.localPosition;
            var offset = laneHeight * laneOrder[x] * -1;

            objects[x].transform.localPosition = new Vector3(existing.x, offset + laneHeight, existing.z);
        }
    }
}
