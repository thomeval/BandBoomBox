using UnityEngine;
using UnityEngine.U2D.Animation;

public class NoteHighwayImpactZone : MonoBehaviour
{
    public SpriteResolver TopLaneResolver;
    public SpriteResolver MidLaneResolver;
    public SpriteResolver BottomLaneResolver;

    private const string SPRITE_CATEGORY = "ImpactZoneGlyphs";
    public void SetSprites(Difficulty playerDifficulty, bool hasTopLaneNotes)
    {
        var topLaneSprite = hasTopLaneNotes ? "Top-Default" : "None";
        TopLaneResolver.SetCategoryAndLabel(SPRITE_CATEGORY, topLaneSprite);
        var midLaneSprite = playerDifficulty == Difficulty.Beginner ? "Mid-Beginner" : "Mid-Default";
        MidLaneResolver.SetCategoryAndLabel(SPRITE_CATEGORY, midLaneSprite);
        BottomLaneResolver.SetCategoryAndLabel(SPRITE_CATEGORY, "Bot-Default");

    }
}
