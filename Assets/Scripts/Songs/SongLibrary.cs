using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class SongLibrary : MonoBehaviour
    {
        public List<SongData> Songs = new List<SongData>();

        public string[] SongFolders;

        public List<string> UnavailableSongs { get; internal set; } = new();

        public event EventHandler LoadSongsCompleted;

        public SongData this[string id]
        {
            get { return Songs.FirstOrDefault(e => e.ID == id); }
            private set
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }
                var existing = Songs.FirstOrDefault(e => e.ID == id);

                if (existing != null)
                {
                    Songs.Remove(existing);
                }

                Songs.Add(value);
            }
        }

        public void Clear()
        {
            Songs.Clear();
        }
        public void LoadSongs()
        {
            foreach (var folder in SongFolders)
            {
                var temp = Helpers.ResolvePath(folder);
                if (!Directory.Exists(temp))
                {
                    Debug.LogWarning($"Song folder does not exist: {temp}");
                    continue;
                }
                Debug.Log($"Searching for songs in: {temp}");
                LoadSongs(temp);
            }
            Debug.Log($"Finished searching for songs. Loaded {Songs.Count} songs.");
            LoadSongsCompleted?.Invoke(this, null);
        }


        private void LoadSongs(string folder)
        {
            foreach (var subfolder in Directory.GetDirectories(folder))
            {
                LoadSongs(subfolder);
            }

            foreach (var file in Directory.GetFiles(folder, "*.sjson"))
            {
                LoadSong(file);
            }
        }
        public SongData LoadSong(string path, bool ignoreDuplicate = false)
        {

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var folder = Path.GetDirectoryName(path);
            var json = File.ReadAllText(path);
            //  var song = JsonUtility.FromJson<SongData>(json);
            var song = JsonConvert.DeserializeObject<SongData>(json);

            if (this.Contains(song.ID) && !ignoreDuplicate)
            {
                var otherPath = this[song.ID].SjsonFilePath;
                Debug.LogWarning($"Found duplicate song with ID {song.ID} at {path} and {otherPath}.");
            }

            song.SjsonFilePath = path;
            song.AudioPath = Path.Combine(folder, song.AudioFile);
            song.Sections = song.Sections.OrderBy(e => e.Key).ToDictionary(e => e.Key, e => e.Value);
            CheckSongChartNoteCounts(song);
            this[song.ID] = song;
            return song;
        }

        private void CheckSongChartNoteCounts(SongData songData)
        {
            var changed = false;
            foreach (var chart in songData.SongCharts)
            {
                changed |= CheckSongChartNoteCounts(chart);
            }

            if (changed)
            {
                Debug.Log($"Updated note counts for song ({songData.ID}) - {songData.Artist} - {songData.Title}");
                this.SaveSongToDisk(songData);
            }
        }

        private bool CheckSongChartNoteCounts(SongChart chart)
        {
            if (chart.NoteCounts.TotalNotes != 0)
            {
                return false;
            }

            chart.NoteCounts = NoteCounter.CountNotes(chart);
            return chart.NoteCounts.TotalNotes > 0;
        }

        public void SaveSongToDisk(SongData songData)
        {
            try
            {
                var filePath = songData.SjsonFilePath;
                var folder = Path.GetDirectoryName(songData.SjsonFilePath);

                if (Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var json = JsonConvert.SerializeObject(songData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Debug.Log($"Successfully saved SJSON file: {filePath}");

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public bool Contains(SongData songData)
        {
            if (songData == null)
            {
                throw new ArgumentNullException();
            }

            return Contains(songData.ID);
        }

        public bool Contains(string songId)
        {
            if (string.IsNullOrEmpty(songId))
            {
                return false;
            }

            return Songs.Any(e => e.ID == songId);
        }

        public bool Contains(string songId, int version)
        {
            if (string.IsNullOrEmpty(songId))
            {
                return false;
            }

            return Songs.Any(e => e.ID == songId && e.Version == version);
        }

        public bool Contains(NetSongChoiceRequest request)
        {
            return Contains(request.SongId, request.SongVersion);
        }

        public void Add(SongData songData)
        {
            Songs.Add(songData);
        }

    }
}
