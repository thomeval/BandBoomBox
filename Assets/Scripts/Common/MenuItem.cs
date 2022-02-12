using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public bool IsSelectable;

    public bool IsShiftable;

    public string Text;

    [UnityEngine.Multiline] 
    public string Explanation;
}
