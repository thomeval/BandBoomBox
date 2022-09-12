using UnityEngine;
using UnityEngine.U2D.Animation;

public class Note : MonoBehaviour
{
    public float Position;
    public float AbsoluteTime;

    public NoteType NoteType;
    public NoteClass NoteClass;
    public Note EndNote;

    public GameObject LabelObject;

    public SpriteRenderer HoldTailSprite;
    public int HoldSpriteHeight = 100;

    public int Lane;

    public float MxValue = 0;

    private SpriteResolver _spriteResolver;
    private SpriteResolver _holdSpriteResolver;
    private SpriteResolver _labelSpriteResolver;

    void Awake()
    {
        _spriteResolver = GetComponent<SpriteResolver>();
        _labelSpriteResolver = LabelObject.GetComponent<SpriteResolver>();
    }

    public void CalculateTailWidth()
    {
        if (HoldTailSprite == null || EndNote == null)
        {
            return;
        }

        var width = EndNote.transform.localPosition.x - this.transform.localPosition.x;
        width /= this.transform.localScale.x;

        HoldTailSprite.size = new Vector2(width, HoldSpriteHeight);
        HoldTailSprite.transform.localPosition = new Vector3(width / 2, 0, 0);
    }
    public void SetPosition(float newXPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos, currentPos.y, currentPos.z);
    }

    public void SetPosition(float newXPos, float newYPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos, newYPos, currentPos.z);
    }

    public void SetSpriteCategories(string noteSkinCat, string labelSkinCat)
    {
        if (_spriteResolver == null)
        {
            Awake();
        }

        if (HoldTailSprite != null && _holdSpriteResolver == null)
        {
            _holdSpriteResolver = HoldTailSprite.GetComponent<SpriteResolver>();
        }

        if (this.NoteClass == NoteClass.Release)
        {
            noteSkinCat = "HoldTailEnds";
            LabelObject.SetActive(false);
        }
        else
        {
            noteSkinCat = "Notes_" + noteSkinCat;
            LabelObject.SetActive(true);
        }

        _spriteResolver.SetCategoryAndLabel(noteSkinCat, NoteType.ToString());
        _labelSpriteResolver.SetCategoryAndLabel("Labels_" + labelSkinCat, NoteType.ToString());
        _holdSpriteResolver?.SetCategoryAndLabel("HoldTails", NoteType.ToString());
    }

    public void RefreshSprites()
    {
        if (_spriteResolver == null)
        {
            Awake();
        }

        if (HoldTailSprite != null && _holdSpriteResolver == null)
        {
            _holdSpriteResolver = HoldTailSprite.GetComponent<SpriteResolver>();
        }

        _spriteResolver.SetCategoryAndLabel(_spriteResolver.GetCategory(), NoteType.ToString());
        _labelSpriteResolver.SetCategoryAndLabel(_labelSpriteResolver.GetCategory(), NoteType.ToString());
        _holdSpriteResolver?.SetCategoryAndLabel(_holdSpriteResolver.GetCategory(), NoteType.ToString());
    }
}

