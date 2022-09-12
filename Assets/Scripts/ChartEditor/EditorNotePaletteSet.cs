using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class EditorNotePaletteSet : MonoBehaviour
{
    public EditorNotePalette[] NotePalettes = new EditorNotePalette[0];
    public GameObject PaletteContainer;

    [SerializeField]
    private Difficulty displayedPalette = Difficulty.Beginner;

    public Difficulty DisplayedPalette
    {
        get
        {
            return displayedPalette;
        }
        set
        {
            displayedPalette = value;
            DisplayPalette();
        }
    }

    void Awake()
    {
        NotePalettes = PaletteContainer.GetComponentsInChildren<EditorNotePalette>(true);
    }

    public void SetNoteSkin(string noteSkin, string labelSkin)
    {
        foreach (var palette in NotePalettes)
        {
            palette.SetNoteskin(noteSkin, labelSkin);
        }
    }

    public void DisplayPalette()
    {
        foreach (var palette in NotePalettes)
        {
            palette.Hide();
        }

        var paletteToShow = NotePalettes.Single(e => e.Difficulty == DisplayedPalette);
        paletteToShow.Show();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (NotePalettes.Length == 0)
        {
            Awake();
        }

        DisplayPalette();
    }
}
