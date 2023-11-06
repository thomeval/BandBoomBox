using System.Linq;
using UnityEngine;

public class EditorNotePaletteSet : MonoBehaviour
{
    public EditorNotePalette[] NotePalettes = new EditorNotePalette[0];
    public GameObject PaletteContainer;

    [SerializeField]
    private Difficulty _displayedPalette = Difficulty.Beginner;

    public Difficulty DisplayedPalette
    {
        get
        {
            return _displayedPalette;
        }
        set
        {
            _displayedPalette = value;
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
            var isEnabled = palette.gameObject.activeSelf;
            palette.gameObject.SetActive(true);
            palette.SetNoteskin(noteSkin, labelSkin);
            palette.gameObject.SetActive(isEnabled);
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
}
