using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayStateRecorder : MonoBehaviour
{
    [SerializeField]
    private float _beatsPerSnapshot;
    [SerializeField]
    private float _songEndPosition;

    private List<GameplayStateValuesDto> _snapshots = new();

    public int SnapshotsPerSong = 20;
    public float NextSnapshotBeat = 0.0f;

    public bool NeedsUpdate(float songPosition)
    {
        return NextSnapshotBeat <= songPosition;
    }

    public void Init(float songEndPosition)
    {
        _snapshots = new();
        _beatsPerSnapshot = songEndPosition / SnapshotsPerSong;
        NextSnapshotBeat = _beatsPerSnapshot;
    }

    public void Add(float songPosition, GameplayStateValues values)
    {
        var dto = values.AsDto();
        dto.SongPosition = songPosition;
        _snapshots.Add(dto);
        NextSnapshotBeat += _beatsPerSnapshot;
    }

    public float[] GetMultipliers()
    {
        return _snapshots.OrderBy(e => e.SongPosition).Select(s => (float)s.Multiplier).ToArray();
    }
}
