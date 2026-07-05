using System.Linq;
using Unity.Netcode;

/// <summary>
/// Represents the favourite songs of all players in a network session, organized by machine and player slot. This class is used for synchronizing favourite song data across the network.
/// </summary>
public class NetworkSessionFavouriteSongSet : INetworkSerializable
{
    public NetworkMachineFavouriteSongSet[] Machines = new NetworkMachineFavouriteSongSet[0];
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Machines);
    }

    public static NetworkSessionFavouriteSongSet FromPlayers(Player[] players)
    {
        var machines = players
            .GroupBy(p => p.NetId)
            .Select(g => NetworkMachineFavouriteSongSet.FromPlayers(g.Key, g.ToArray()))
            .ToArray();
        return new NetworkSessionFavouriteSongSet
        {
            Machines = machines
        };
    }
}

/// <summary>
/// Represents the favourite songs of all players on a specific machine in a network session. This class contains the machine's network ID and an array of player favourite song sets.
/// </summary>
public class NetworkMachineFavouriteSongSet: INetworkSerializable
{
    public ulong NetId;
    public NetworkPlayerFavouriteSongSet[] Players = new NetworkPlayerFavouriteSongSet[0];

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref Players);
    }

    public static NetworkMachineFavouriteSongSet FromPlayers(ulong netId, Player[] players)
    {
        return new NetworkMachineFavouriteSongSet
        {
            NetId = netId,
            Players = players.Select(p => NetworkPlayerFavouriteSongSet.FromProfileData(p.Slot, p.ProfileData)).ToArray()
        };
    }
}

/// <summary>
/// Represents the favourite songs of a specific player (on a specific remote machine) in a network session. This class contains the player's slot number and an array of their favourite songs.
/// </summary>
public class NetworkPlayerFavouriteSongSet : INetworkSerializable
{
    public int PlayerSlot;
    public SongLibraryEntry[] FavouriteSongs = new SongLibraryEntry[0];

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerSlot);
        serializer.SerializeValue(ref FavouriteSongs);
    }

    public static NetworkPlayerFavouriteSongSet FromProfileData(int playerSlot, ProfileData profileData)
    {
        return new NetworkPlayerFavouriteSongSet
        {
            PlayerSlot = playerSlot,
            FavouriteSongs = profileData.FavouriteSongs.Select(fav => new SongLibraryEntry { ID = fav }).ToArray()
        };
    }
}