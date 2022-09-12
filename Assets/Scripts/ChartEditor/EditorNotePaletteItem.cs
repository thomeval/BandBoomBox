using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

[ExecuteAlways]
public class EditorNotePaletteItem : MonoBehaviour
{
    public string Label;
    public NoteType NoteType;

    public Text TxtLabel;
    public Note Note;

    public void SetNoteskin(string noteSkin, string labelSkin)
    {
        Note.SetSpriteCategories(noteSkin, labelSkin);
    }

    private void Awake()
    {      
        if (Note == null || TxtLabel == null)
        {
            return;
        }

        Note.NoteType = this.NoteType;
        Note.RefreshSprites();
        TxtLabel.text = Label;
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        Awake();     
    }
}
