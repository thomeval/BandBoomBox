using System.Linq;
using UnityEngine;

public class EditorNotePalette : MonoBehaviour
{
    public Difficulty Difficulty;

    public EditorNotePaletteItem[] PaletteItems = new EditorNotePaletteItem[0];

    private void Awake()
    {
        PaletteItems = this.gameObject.GetComponentsInChildren<EditorNotePaletteItem>();
    }

    public void SetNoteskin(string noteSkin, string labelSkin)
    {
        if (!PaletteItems.Any())
        {
            Awake();
        }

        foreach (var item in PaletteItems)
        {
            item.SetNoteskin(noteSkin, labelSkin);
        }
    }

}
