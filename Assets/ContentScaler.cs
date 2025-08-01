using UnityEngine;

[ExecuteAlways]
public class ContentScaler : MonoBehaviour
{
    public float PreferredWidth;
    public float PreferredHeight;

    private RectTransform _transform;
    void Update()
    {
        if (_transform == null)
        {
            _transform = GetComponent<RectTransform>();
        }

        if (PreferredHeight <= 0.0f)
        {
            PreferredHeight = _transform.rect.height;
        }

        if (PreferredWidth <= 0.0f)
        {
            PreferredWidth = _transform.rect.width;
        }

        var currentWidth = _transform.rect.width;
        var currentHeight = _transform.rect.height;

        var widthScale = PreferredWidth / currentWidth;
        var heightScale = PreferredHeight / currentHeight;

        if (widthScale < heightScale)
        {
            transform.localScale = new Vector3(widthScale, widthScale, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(heightScale, heightScale, 1.0f);
        }

    }
}
