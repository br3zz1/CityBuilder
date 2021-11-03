using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public static ToolController Instance;

    public object hoveringOver;

    public IUseable useable;

    public TileObject tileObjectBuildPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        useable?.Tick();
    }
}

public interface IUseable
{
    public bool Use();
    public void Tick();
}
