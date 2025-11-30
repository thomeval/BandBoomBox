using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RebindManager : MonoBehaviour
{
    private Action<string> _onComplete;

    [SerializeField]
    private bool _isListening;

    public void StartListening(Action<string> onComplete)
    {
        _onComplete = onComplete;
        _isListening = true;
    }

    void Update()
    {
        if (!_isListening)
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
        _isListening = false;
        _onComplete?.Invoke(path);
        _onComplete = null;
    }
}
