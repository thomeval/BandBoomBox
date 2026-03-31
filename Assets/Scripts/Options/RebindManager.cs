using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RebindManager : MonoBehaviour
{
    private Action<string> _onComplete;

    public bool IsListening;

    public void StartListening(Action<string> onComplete)
    {
        _onComplete = onComplete;
        IsListening = true;
    }

    void Update()
    {
        if (!IsListening)
        {
            return;
        }

        if (Keyboard.current == null || !Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return;
        }

        foreach (KeyControl key in Keyboard.current.allKeys.Where(e => e.wasPressedThisFrame))
        {
            // Escape key to cancel
            if (key.keyCode == Key.Escape)
            {
                Complete(null);
                return;
            }

            string path = key.path;
            string formattedPath = "<Keyboard>/" + key.name;
            Complete(formattedPath);
            return;

        }
    }

    private void Complete(string path)
    {
        IsListening = false;
        _onComplete?.Invoke(path);
        _onComplete = null;
    }
}
