
using UnityEngine;

public class CustomBindingDisplay : MonoBehaviour
{

    public void Display(CustomBindingSet bindings)
    {
        foreach (var item in GetComponentsInChildren<CustomBindingDisplayItem>())
        {
            item.Display(bindings);
        }
    }
}
