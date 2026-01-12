using System;
using System.Collections.Generic;
using System.Linq;

public class NetworkSongLibrarySet
{
    public Dictionary<ulong, NetworkPlayerSongLibrary> PlayerSongLibraryList = new Dictionary<ulong, NetworkPlayerSongLibrary>();

    public NetworkPlayerSongLibrary GetCommonSongs()
    {
        var result = new NetworkPlayerSongLibrary();
        if (PlayerSongLibraryList.Count == 0)
        {
            return result;
        }

        IEnumerable<SongLibraryEntry> commonSongs = PlayerSongLibraryList.Values.First().Songs;

        foreach (var playerLibrary in PlayerSongLibraryList.Values.Skip(1))
        {
            commonSongs = commonSongs.Intersect(playerLibrary.Songs);
        }

        result.Songs = commonSongs.ToArray();
        return result;
    }

    public void Add(ulong netId, NetworkPlayerSongLibrary networkPlayerSongLibrary)
    {
        PlayerSongLibraryList[netId] = networkPlayerSongLibrary;
    }

    public void Remove(ulong netId)
    {
        PlayerSongLibraryList.Remove(netId);
    }

    public void Clear()
    {
        PlayerSongLibraryList.Clear();
    }
}