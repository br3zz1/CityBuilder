using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommand
{

    public string name { get; private set; }
    public DebugCommandDelegate action { get; private set; }

    public DebugCommand(string name, DebugCommandDelegate action)
    {
        this.name = name;
        this.action = action;
    }

    public void Invoke(string[] args)
    {
        action.Invoke(args);
    }

}

public delegate void DebugCommandDelegate(string[] args);