
using System;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

[DebuggerDisplay("{Position} = {Sjson}")]
public class NoteSjsonEntry
{
    public NoteSjsonEntry()
    {
    }

    public NoteSjsonEntry(Note note)
    {
        if (note == null)
        {
            throw new ArgumentNullException(nameof(note));
        }

        this.Position = note.Position;
        this.Sjson = note.ToSjson();
    }

    public float Position { get; set; }

    public float PositionFraction
    {
        get
        {
            return this.Position - (int)this.Position;
        }
    }

    public string Sjson { get; set; }

    public NoteSjsonEntry Merge(NoteSjsonEntry other)
    {
        
        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (this.Position != other.Position)
        {
            throw new ArgumentException(
                "Merge can only be called on two NoteSjsonEntries that have the same position.");
        }
        var newSjson = new StringBuilder(this.Sjson);

        for (var x = 0; x < newSjson.Length; x++)
        {
            if (newSjson[x] != '0' && other.Sjson[x] != '0' && newSjson[x] != other.Sjson[x])
            {
                Debug.LogWarning($"Attempted to merge two or more incompatible notes at position {this.Position}. This will cause one note to be dropped in the resulting SJSON.");
            }

            newSjson[x] = Max(this.Sjson[x], other.Sjson[x]);
        }
        return new NoteSjsonEntry { Position = this.Position, Sjson = this.Sjson };
    }

    private char Max(char c1, char c2)
    {
        if (c1 >= c2)
        {
            return c1;
        }

        return c2;
    }
}

