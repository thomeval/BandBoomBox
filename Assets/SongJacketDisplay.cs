using System;
using System.IO;
using UnityEngine;

public class SongJacketDisplay : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        if (SpriteRenderer == null)
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void DisplayJacket(SongData songData)
    {
        var songDataFolder = Path.GetDirectoryName(songData.SjsonFilePath);
        var jacketPath = Path.Combine(songDataFolder, songData.AlbumJacketArtFile);

        if (string.IsNullOrEmpty(jacketPath) || !File.Exists(jacketPath))
        {
            SpriteRenderer.sprite = null;
            return;
        }

        try
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(jacketPath));
            SpriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }
        catch (Exception ex)
        {
            Debug.LogWarning ($"Failed to load jacket art for song {songData.Title}: {ex.Message}");
        }
    }

}
