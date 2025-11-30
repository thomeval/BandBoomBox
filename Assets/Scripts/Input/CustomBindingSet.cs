using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CustomBindingSet
{
    public int Version { get; set; }
    public List<CustomBinding> Bindings { get; set; }

    public int MAX_BINDINGS_FOR_ACTION = 2;
    public CustomBinding[] GetBindingsByAction(string action)
    {
        return Bindings.Where(b => b.Action == action).ToArray();
    }

    public int CountBindings(string action)
    {
        return Bindings.Count(b => b.Action == action);
    }

    public CustomBinding GetBindingByKey(string key)
    {
        return Bindings.FirstOrDefault(b => b.Path == key);
    }

    public void UnbindKey(string key)
    {
        var binding = GetBindingByKey(key);
        if (binding != null)
        {
            Bindings.Remove(binding);
        }
    }

    public void UnbindAction(string action)
    {
        Bindings.RemoveAll(b => b.Action == action);
    }

    public void BindKey(string action, string key)
    {
        var binding = GetBindingByKey(key);
        if (binding != null)
        {
            binding.Action = action;
            return;
        }

        Bindings.Add(new CustomBinding { Action = action, Path = key });
        EnforceMaxBindings(action);
    }

    private void EnforceMaxBindings(string action)
    {
        var currentBindings = Bindings.Where(b => b.Action == action).ToList();

        for (int x = currentBindings.Count - 1; x >= MAX_BINDINGS_FOR_ACTION; x--)
        {
            var binding = currentBindings[x];
            Bindings.Remove(binding);
        }
    }
}