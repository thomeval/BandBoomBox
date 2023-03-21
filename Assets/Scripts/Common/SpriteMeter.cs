using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteAlways]
public class SpriteMeter : MonoBehaviour
{

    private Transform _innerTransform;

    private SpriteRenderer _innerSprite;

    public SpriteRenderer InnerSprite
    {
        get { return _innerSprite; }
    }

    [SerializeField]
    [Range(0.0f,1.0f)]
    private float _value;
    public float Value 
    { get
        {
            return _value;
        }
        set
        {
            _value = value;
            ResizeSprite();
        }
    }

    private void ResizeSprite()
    {
        try
        {
            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }
            if (_innerSprite == null || _innerTransform == null)
            {
                Debug.LogWarning("Inner sprite or transform is missing from SpriteMeter.");
                return;
            }

            float offset;
            switch (SpriteMeterMode)
            {
                case SpriteMeterMode.BottomToTop:
                    offset = Value * MaxSize.y / 2;
                    if (float.IsNaN(offset))
                    {
                        Debugger.Break();
                    }

                    _innerSprite.size = new Vector2(MaxSize.x, MaxSize.y * Value);
                    _innerTransform.localPosition = new Vector3(0, offset);
                    break;
                case SpriteMeterMode.LeftToRight:
                    _innerSprite.size = new Vector2(MaxSize.x * Value, MaxSize.y);
                    offset = Value * MaxSize.x / 2;
                    _innerTransform.localPosition = new Vector3(offset, 0);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }



    }

    public Vector2 MaxSize = new Vector2(1,1);

    public SpriteMeterMode SpriteMeterMode = SpriteMeterMode.BottomToTop;

    void Awake()
    {
        _innerTransform = this.transform.GetChild(0);
        _innerSprite = _innerTransform.gameObject.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResizeSprite();   
    }

}

public enum SpriteMeterMode
{
    BottomToTop,
    LeftToRight
}
