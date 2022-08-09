using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class BackgroundPlaneAnimator : MonoBehaviour
{
    public float BaseSpeed = 3.0f;
    public float SpeedMultiplier = 1;
    private float _loopPoint;

    public GameObject StripPrefab;
    public GameObject StripContainer;
    public List<GameObject> Strips = new();

    public int StripsToRender = 100;
    public Vector2 StripStepSize = new(5, 6.25f);

    [SerializeField]
    private double _beatNumber;

    public double BeatNumber
    {
        get
        {
            return _beatNumber;
        }
        set
        {
            _beatNumber = value;
        }
    }

    private DateTime _lastUpdate;
    
    void Awake()
    {
        _lastUpdate = DateTime.Now;
        _loopPoint = StripStepSize.y * StripsToRender / 2;
        if (StripContainer.transform.childCount > 0)
        {
            return;
        }
        
        GenerateStrips();
    }
    void FixedUpdate()
    {
        var deltaSeconds = (DateTime.Now - _lastUpdate).TotalSeconds;
        var deltaMove = (float) (BaseSpeed * SpeedMultiplier * deltaSeconds);

        var newX = (int) BeatNumber * 2 * StripStepSize.x;
        var newZ = StripContainer.transform.position.z - deltaMove;
        
        if (Math.Abs(newZ) > Mathf.Abs(_loopPoint))
        {
            newZ += _loopPoint;
        }

        StripContainer.transform.position = new Vector3(newX, 0, newZ);
        _lastUpdate = DateTime.Now;
    }

    private void GenerateStrips()
    {
        Strips.Clear();
        
        for (int x = 0; x < StripsToRender; x++)
        {
            var strip = Instantiate(StripPrefab);
            var offset = Random.Range(-8, 8);
            offset *= 2;
            if (x % 2 == 0)
            {
                offset ++;
            }
            var xPos = StripStepSize.x * offset;
            var zPos = x * StripStepSize.y;
            strip.transform.position = new Vector3(xPos, 0, zPos);
            strip.transform.parent = StripContainer.transform;
            
            Strips.Add(strip);
        }
    }
}
