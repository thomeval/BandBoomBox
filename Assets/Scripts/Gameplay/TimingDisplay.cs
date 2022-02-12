using UnityEngine;
using UnityEngine.U2D.Animation;

public class TimingDisplay : MonoBehaviour
{
    // Start is called before the first frame update

    public const int LANE_COUNT = 3;

    public GameObject[] LaneSprites = new GameObject[LANE_COUNT];
    public string SpriteCategory;

    private SpriteFader[] _spriteFaders;
    private SpriteResolver[] _spriteResolvers;

    void Awake()
    {
        _spriteFaders = new SpriteFader[LaneSprites.Length];
        _spriteResolvers = new SpriteResolver[LaneSprites.Length];

        for (int x = 0; x < LaneSprites.Length; x++)
        {
            _spriteResolvers[x] = LaneSprites[x].GetComponent<SpriteResolver>();
            _spriteFaders[x] = LaneSprites[x].GetComponent<SpriteFader>();
        }
    }

    public void ShowHit(HitResult hitResult)
    {

        var label = string.Format("{0}{1}", hitResult.JudgeResult, hitResult.DeviationResult).Replace("NotHit","");
        _spriteResolvers[hitResult.Lane].SetCategoryAndLabel(SpriteCategory,label);
        _spriteFaders[hitResult.Lane].Reset();
    }
}
