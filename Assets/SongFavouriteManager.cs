using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongFavouriteManager : MonoBehaviour
{
    public SongLibrary SongLibrary;
    public PlayerManager PlayerManager;

    private Dictionary<string, SongFavouritesEntry> _favourites = new Dictionary<string, SongFavouritesEntry>();

    /// <summary>
    /// Builds the list of SongFavouritesEntry objects for all songs in the SongLibrary and stores them in the _favourites dictionary. This method should be called whenever the song library is updated or when the favorite status of any song changes.
    /// </summary>
    public void BuildFavouritesList()
    {
        _favourites.Clear();
        foreach (var song in SongLibrary.Songs)
        {
            _ = RefreshEntry(song.ID);
        }

    }

    /// <summary>
    /// Refreshes the SongFavouritesEntry for the specified song ID by rebuilding it and updating the cache. If the song ID does not exist in the cache, a new entry will be created.
    /// This method should be called whenever the favorite status of a song changes for a player, or when the players in the current session changes.
    /// </summary>
    /// <param name="songId">The ID of the song for which to refresh favorite information.</param>
    /// <returns>A SongFavouritesEntry object containing updated favorite information for the specified song.</returns>
    public SongFavouritesEntry RefreshEntry(string songId)
    {
        var entry = BuildFavouritesEntryForSong(songId);
        _favourites[songId] = entry;
        return entry;
    }

    /// <summary>
    /// Returns a SongFavouritesEntry object from the cache that contains information about which local players have marked the specified song as a favorite, as well as the number of remote players who have done so during Network Games.
    /// </summary>
    /// <param name="songId">The ID of the song for which to retrieve favorite information.</param>
    /// <returns>A SongFavouritesEntry object containing favorite information for the specified song.</returns>
    public SongFavouritesEntry GetFavouritesEntryForSong(string songId)
    {
        if (!_favourites.ContainsKey(songId))
        {
            return RefreshEntry(songId);
        }
        return _favourites[songId];
    }

    /// <summary>
    /// Constructs a SongFavouritesEntry object that contains information about which local players have marked the specified song as a favorite, as well as the number of remote players who have done so during Network Games.
    /// </summary>
    /// <param name="songId">The ID of the song for which to retrieve favorite information.</param>
    /// <returns>A SongFavouritesEntry object containing favorite information for the specified song.</returns>
    public SongFavouritesEntry BuildFavouritesEntryForSong(string songId)
    {
        var entry = new SongFavouritesEntry
        {
            SongId = songId,
            IsLocalFavorite = new bool[8],
            RemoteFavoriteCount = 0
        };

        foreach (var player in PlayerManager.GetLocalPlayers())
        {
            entry.IsLocalFavorite[player.LocalSlot - 1] = player.IsFavouriteSong(songId);        
        }

        foreach (var player in PlayerManager.GetRemotePlayers())
        {
            if (player.IsFavouriteSong(songId))
            {
                entry.RemoteFavoriteCount++;
            }
        }
        return entry;

    }

    public int Count(string songId)
    {
        var entry = GetFavouritesEntryForSong(songId);
        return entry.TotalCount;
    }
}

