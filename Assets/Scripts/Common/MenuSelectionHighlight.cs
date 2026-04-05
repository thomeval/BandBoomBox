using UnityEngine;

public class MenuSelectionHighlight : MonoBehaviour
{
    public Vector2 Padding = new Vector2();

    private RectTransform _myTransform;
    private SpriteRenderer _myRenderer;

    public Color Color
    {
        get => _myRenderer.color;
        set
        {
            if (_myRenderer == null)
            {
                Awake();
            }
            _myRenderer.color = value;
        }
    }

    public void HighlightObject(GameObject obj)
    {

        var rt = obj.GetComponent<RectTransform>();

        if (rt == null || _myTransform == null || _myRenderer == null)
        {
            return;
        }

        _myTransform.position = rt.position;
        _myTransform.sizeDelta = rt.sizeDelta;
        _myRenderer.size = rt.sizeDelta + Padding;
    }

    public void Awake()
    {
        _myTransform = GetComponent<RectTransform>();
        _myRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }
}
