using UnityEngine;

[ExecuteInEditMode]
public class SpriteScaler : MonoBehaviour
{
    private RectTransform _rectTransform;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ScaleSprite();
    }

    private void ScaleSprite()
    {
        var spriteSize = _spriteRenderer.size;
        var transformSize = _rectTransform.rect.size;

        if (spriteSize == transformSize)
        {
            return;
        }

        _spriteRenderer.size = transformSize;
    }
}
