using System;
using System.Collections.Generic;

[Serializable]
public class CustomBindingSet
{
    public int Version { get; set; }
    public List<CustomBinding> Bindings { get; set; }
}