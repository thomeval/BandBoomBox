using UnityEngine;
using UnityEngine.U2D.Animation;

public class SectionResultDisplay : MonoBehaviour
{
    [SerializeField]
    private SpriteResolver _spriteResolver;

    [SerializeField]
    private Animator _animator;

    public void ShowSectionResult(SectionJudgeResult result)
    {             
        _spriteResolver.SetCategoryAndLabel("SectionResults", result.ToString());
        _animator.Play("ShowResult");
    }
}
