using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ProfileManager : MonoBehaviour
{
    public string ProfilesPath = "%AppSaveFolder%/Profiles";
    public List<ProfileData> Profiles = new List<ProfileData>();

    public ProfileData this[string id]
    {
        get { return Profiles.SingleOrDefault(e => e.ID == id); }
        set
        {
            var existing = Profiles.SingleOrDefault(e => e.ID == id);

            if (existing != null)
            {
                Profiles.Remove(existing);
            }

            Profiles.Add(value);
        }
    }

    public ProfileData GuestProfile
    {
        get
        {
            return new ProfileData
            {
                Exp = 0,
                Goal = null,
                ID = null,
                Name = "Guest",
                ScrollSpeed = 500,
                TimingDisplayType = TimingDisplayType.Words,
                MistakeSfxEnabled = true,
                RumbleEnabled = true
            };
        }
    }

    private readonly string[] _invalidProfileNames = { "##NEW##", "GUEST", "" };
    public ProfileData Create(string profileName)
    {
        if (this.ContainsName(name))
        {
            throw new ArgumentException("A profile with the given name is invalid or already exists.");
        }

        var result = GuestProfile;
        result.ID = Guid.NewGuid().ToString();
        result.Name = profileName;
        result.LastPlayed = DateTime.Now;
        this[result.ID] = result;
        return result;
    }

    public bool ContainsName(string profileName)
    {
        if (_invalidProfileNames.Any(e => string.Equals(e, profileName, StringComparison.InvariantCultureIgnoreCase)))
        {
            return true;
        }

        return Profiles.Any(e => string.Equals(e.Name, profileName, StringComparison.InvariantCultureIgnoreCase));
    }

    public ProfileData GetProfileWithName(string profileName)
    {
        return Profiles.SingleOrDefault(e => string.Equals(e.Name, profileName, StringComparison.InvariantCultureIgnoreCase));
    }

    public ProfileData GetLeadRhythmist()
    {
        var result = Profiles.OrderByDescending(e => e.Exp).FirstOrDefault();

        if (result.Name == "Guest" || result.Exp == 0)
        {
            return null;
        }

        return result;
    }

    public void Load()
    {
        var path = Helpers.ResolvePath(ProfilesPath);
        if (!Directory.Exists(path))
        {
            Debug.LogWarning($"Failed to load profiles from {path}. Directory not found.");
            return;
        }

        foreach (var file in Directory.GetFiles(path, "*.prf"))
        {
            LoadProfile(file);
        }

        Profiles = Profiles.OrderBy(e => e.Name).ToList();
        Debug.Log($"Finished loading {Profiles.Count} profiles from {path}.");
    }

    public void Save(ProfileData data)
    {
        if (data.ID == null)
        {
            throw new ArgumentException("Attempted to save a profile with an invalid ID. ");
        }

        this[data.ID] = data;
        Save(data.ID);
    }
    public void Save(string id)
    {
        var path = Helpers.ResolvePath(ProfilesPath);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var profile = this[id];

        path = Path.Combine(path, profile.Name) + ".prf";

        try
        {
            var json = JsonConvert.SerializeObject(profile);
            CompressedFileHelper.CompressToFile(Path.Combine(path), json);
            Debug.Log($"Successfully saved profile to {path}.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to save profile to {path}. {ex.Message}");
        }
    }

    private void LoadProfile(string path)
    {

        try
        {
            var json = CompressedFileHelper.DecompressFromFile(path);
            var profile = JsonConvert.DeserializeObject<ProfileData>(json);
            this[profile.ID] = profile;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to load profile from {path}. {ex.Message}");
        }
    }

    public PlayerScore GetPlayerHighScore(string profileId, string songId, int songVersion, Difficulty difficulty, string chartGroup)
    {
        var profile = this[profileId];

        if (profile == null)
        {
            return null;
        }

        return profile.GetPlayerHighScore(songId, songVersion, difficulty, chartGroup);
    }

    public PlayerScore GetBestPlayerHighScore(string profileId, string songId, int songVersion)
    {
        var profile = this[profileId];

        if (profile == null)
        {
            return null;
        }

        return profile.GetBestPlayerHighScore(songId, songVersion);
    }

    public bool SavePlayerScore(Player player, string songId, int songVersion)
    {
        if (player.ProfileId == null)
        {
            return false;
        }
        var profile = this[player.ProfileId];

        var playerScore = player.GetPlayerScore(songId, songVersion);
        return profile.AddPlayerScore(playerScore);
    }

}

