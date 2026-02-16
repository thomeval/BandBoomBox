using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SectionResultDisplay : MonoBehaviour
{
    [SerializeField]
    private SpriteResolver _spriteResolver;

    [SerializeField]
    private SpriteResolver _fullComboSpriteResolver;

    [SerializeField]
    private Animator _animator;

    public void ShowSectionResult(SectionJudgeResult result)
    {             
        _spriteResolver.SetCategoryAndLabel("SectionResults", result.ToString());
        _animator.Play("ShowResult");
    }

    public void ShowFullComboResult(FullComboType fullComboType)
    {
        _fullComboSpriteResolver.SetCategoryAndLabel("NetFullComboResults", fullComboType.ToString());
        _animator.Play("ShowFullComboResult");
    }
}
