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

    [SerializeField]
    private SpriteMask _spriteMask;

    [SerializeField]
    private SpriteRenderer[] _spriteRenderers = Array.Empty<SpriteRenderer>();

    public void SetIndex(int index)
    {
        _spriteMask.frontSortingOrder = index;
        _spriteMask.backSortingOrder = index - 1;
        foreach (var spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.sortingOrder = index;
        }
    }

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
